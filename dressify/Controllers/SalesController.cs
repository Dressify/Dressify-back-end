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
            return Ok(product);
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
            var salesProducts = await _unitOfWork.Product.FindAllAsync(p=>p.Vendor.StoreName==SD.StoreName, skip, PageSize, new[] { "ProductImages" });
            var count = await _unitOfWork.Product.CountAsync(p => p.Vendor.StoreName == SD.StoreName);

            if (!salesProducts.Any())
                return NoContent();
            return Ok(new { Count = count, SalesProducts = salesProducts });
        }

        [HttpPut("AddQuantity")]
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
            var questions = await _unitOfWork.ProductQuestion.FindAllAsync(p => p.Product.Vendor.StoreName == SD.StoreName, skip, PageSize, new[] { "Product" });
            var count = await _unitOfWork.ProductQuestion.CountAsync(p => p.Product.Vendor.StoreName == SD.StoreName);

            if (!questions.Any())
            {
                return NoContent();
            }
            return Ok(new { Count = count, Questions = questions });
        }

        [HttpGet("GetQuestionById")]
        public async Task<ActionResult> GetQuestionById([FromQuery] int questionId)
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.GetUserAsync(uId);
            if (vendor == null)
            {
                return NotFound("vendor does not exist");
            }
            var question = await _unitOfWork.ProductQuestion.FindAsync(u => u.QuestionID == questionId && u.Product.Vendor.StoreName == SD.StoreName, new[] { "Product" });

            if (question == null)
            {
                return NoContent();
            }
            return Ok(question);
        }


        [HttpPut("AnswerQuestion")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> AnswerQuestion(AnswerDto obj)
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
            if (question.Product.Vendor.StoreName != SD.StoreName)
            {
                return Unauthorized();
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
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;
            var pendingOrders = await _unitOfWork.OrderDetails.FindAllAsync(od => od.Status == SD.Status_Pending && od.Vendor.StoreName==SD.StoreName, skip, PageSize);
            if(!pendingOrders.Any())
            {
                return NoContent();
            }
            var count = await _unitOfWork.OrderDetails.CountAsync(od => od.Status == SD.Status_Pending && od.Vendor.StoreName == SD.StoreName);
            return Ok(new { Count = count, PendingSalesOrders = pendingOrders });
        }
        [HttpGet("GetSalesOrders")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> GetSalesOrders([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;
            var orders = await _unitOfWork.OrderDetails.FindAllAsync(od =>  od.Vendor.StoreName == SD.StoreName, skip, PageSize);
            if (!orders.Any())
            {
                return NoContent();
            }
            var count = await _unitOfWork.OrderDetails.CountAsync(od => od.Vendor.StoreName == SD.StoreName);
            return Ok(new { Count = count, Orders = orders });
        }
        [HttpGet("GetOrderById")]
        [Authorize(Roles = SD.Role_Sales)]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var Order = _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == orderId && od.Vendor.StoreName==SD.StoreName);
            if (Order == null) return NotFound();
            return Ok(Order);
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
