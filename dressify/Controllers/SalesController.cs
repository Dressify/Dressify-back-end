using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public SalesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("ViewSalesProfile")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> ViewSalesProfile()
        {
            var uId = _unitOfWork.getUID();
            var sales = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);
            var salesProfile = new SalesProfileDto()
            {
                SalesId = uId,
                SalesName = sales.UserName,
                ProfilePic = sales.ProfilePic,
                Email = sales.Email,
                Phone = sales.PhoneNumber,
                FName = sales.FName,
                LName = sales.LName,
                Address = sales.Address,
                NId = sales.NId,
                StoreName = sales.StoreName
            };
            return Ok(salesProfile);
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles =SD.Role_Sales)]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("product Data Not Valid");
            var product = new Product
            {
                VendorId = _unitOfWork.getUID(),
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                Sale = dto.Sale,
                Rentable = dto.Rentable,
                Color = dto.Color,
                Category = dto.Category,
                SubCategory = dto.SubCategory,
                Type = dto.Type,
            };
            await _unitOfWork.Product.AddAsync(product);
            _unitOfWork.Save();
            foreach (var img in dto.Photos)
            {
                CreatePhotoDto result = await _unitOfWork.ProductImage.AddPhoto(img);
                var extension = System.IO.Path.GetExtension(img.FileName);

                var productImg = new ProductImage
                {
                    ProductId = product.ProductId,
                    PublicId = result.PublicId,
                    ImageUrl = result.Url,
                    ImageExtension = extension,
                };
                _unitOfWork.ProductImage.Add(productImg);
                _unitOfWork.Save();
            }
            return Ok();
        }

        [HttpGet("ViewSalesProducts")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> ViewSalesProducts([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;
            var salesUsers = await _userManager.GetUsersInRoleAsync(SD.Role_Sales);
            if (!salesUsers.Any())
            {
                return NoContent();
            }
            var salesProducts = await _unitOfWork.Product.FindAllAsync(p => salesUsers.Any(s => s.Id == p.VendorId && s.StoreName == SD.StoreName), skip, PageSize, new[] { "ProductImages" });
            var count = await _unitOfWork.Product.CountAsync(p => salesUsers.Any(s => s.Id == p.VendorId && s.StoreName == SD.StoreName));

            if (!salesProducts.Any())
                return NoContent();
            return Ok(new { Count = count, SalesProducts = salesProducts });
        }

        [HttpPost("AddQuantity")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> AddQuantity([FromQuery] int productId, [FromQuery] int quantity)
        {
            var product = await _unitOfWork.Product.FindAsync(u => u.ProductId == productId);
            product.Quantity += quantity;
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();
            return Ok(product);
        }

        [HttpGet("GetAllQuestions")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<ActionResult<IEnumerable<ProductQuestion>>> GetAllQuestions([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;

            var salesUsers = await _userManager.GetUsersInRoleAsync(SD.Role_Sales);
            if (!salesUsers.Any())
            {
                return NoContent();
            }
            var questions = await _unitOfWork.ProductQuestion.FindAllAsync(u => salesUsers.Any(s => s.Id == u.Product.VendorId), skip, PageSize, new[] { "Product" });
            var count = await _unitOfWork.ProductQuestion.CountAsync(u => salesUsers.Any(s => s.Id == u.Product.VendorId));

            if (!questions.Any())
            {
                return NoContent();
            }
            return Ok(new { Count = count, Questions = questions });
        }

        [HttpPut("AnswearQuestion")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> AnswearQuestion(AnswerDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(_unitOfWork.getUID());
            var product = await _unitOfWork.Product.GetByIdAsync(obj.ProductId);
            var question = await _unitOfWork.ProductQuestion.GetByIdAsync(obj.QuestionId);

            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            if (question == null)
            {
                return BadRequest("question does not exist");
            }
            if (obj.Answer == null || obj.Answer == "")
            {
                return BadRequest("What's your Answear?!");
            }

            question.Answer = obj.Answer;
            question.VendorId = user.Id;

            _unitOfWork.ProductQuestion.Update(question);
            _unitOfWork.Save();
            return Ok(question);
        }

        [HttpGet("GetPendingSalesOrders")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> GetPendingSalesOrders([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var salesUsers = await _userManager.GetUsersInRoleAsync(SD.Role_Sales);
            var salesIds = salesUsers.Select(s => s.Id).ToArray();
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;
            var pendingOrders = await _unitOfWork.OrderDetails.FindAllAsync(od => od.Status == SD.Status_Pending && salesIds.Contains(od.VendorId), skip, PageSize);
            var count = await _unitOfWork.OrderDetails.CountAsync(od => od.Status == SD.Status_Pending && salesIds.Contains(od.VendorId));
            return Ok(new { Count = count, PendingSalesOrders = pendingOrders });
        }

        [HttpPut("ConfirmPendingOrders")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> ConfirmPendingOrders([FromQuery] int orderId,[FromQuery] int productId)
        {
            var orderDetails = await _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == orderId && od.ProductId == productId);
            if (orderDetails.Status != SD.Status_Pending)
                return BadRequest("order not pended ");
            orderDetails.Status = SD.Status_Confirmed;
            _unitOfWork.OrderDetails.Update(orderDetails);
            _unitOfWork.Save();
            var orders = await _unitOfWork.OrderDetails.FindAllAsync(o => o.OrderId == orderId);
            foreach (var item in orders)
            {
                if (item.Status != SD.Status_Confirmed)
                {
                    break;
                }
                else
                {
                    var order = await _unitOfWork.Order.FindAsync(o => o.OrderId == orderId);
                    order.OrderStatus = SD.Status_Confirmed;
                    _unitOfWork.Order.Update(order);
                    _unitOfWork.Save();
                }
            }
            return Ok();
        }

        
    }
}
