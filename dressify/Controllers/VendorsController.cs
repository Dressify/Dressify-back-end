using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendorsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles =SD.Role_Vendor)]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("product Data Not Valid");
            var product = new Product
            {
                VendorId = _unitOfWork.getUID(),
                ProductName = dto.ProductName.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                Quantity = dto.Quantity,
                Sale = dto.Sale,
                Color = dto.Color.Trim(),
                Category = dto.Category.Trim(),
                SubCategory = dto.SubCategory.Trim(),
                Type = dto.Type.Trim(),
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
        [HttpGet("GetAllQuestions")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<ActionResult<IEnumerable<ProductQuestion>>> GetAllQuestions([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.GetUserAsync(uId);
            if (vendor == null)
            {
                return NotFound("vendor does not exist");
            }
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;

            var questions = await _unitOfWork.ProductQuestion.FindAllAsync(u => u.Product.VendorId == uId && u.Answer == null,skip,PageSize, new[] { "Product" });
            foreach (var question in questions)
            {
                question.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == question.ProductId, new[] { "ProductImages" });
            }
            var count = await _unitOfWork.ProductQuestion.CountAsync(u => u.Product.VendorId == uId && u.Answer == null);

            if (!questions.Any())
            {
                return NoContent();
            }
            return Ok(new { Count = count, Questions = questions });
        }

        [HttpGet("GetQuestionById")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<ActionResult> GetQuestionById([FromQuery]int questionId)
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.GetUserAsync(uId);
            if (vendor == null)
            {
                return NotFound("vendor does not exist");
            }
            var question = await _unitOfWork.ProductQuestion.FindAsync(u => u.QuestionID==questionId&&u.Product.VendorId == uId, new[] { "Product" });
            question.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == question.ProductId, new[] { "ProductImages" });

            if (question==null)
            {
                return NoContent();
            }
            return Ok(question);
        }

        [Authorize(Roles = SD.Role_Vendor)]
        [HttpPut("AnswerQuestion")]
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
            if (question.Product.VendorId != user.Id)
            {
                return Unauthorized();
            }
            question.Answer=obj.Answer;
            question.VendorId=user.Id;

            _unitOfWork.ProductQuestion.Update(question);
            _unitOfWork.Save();
            return Ok(question);
        }

        [Authorize(Roles = SD.Role_Vendor)]
        [HttpGet("GetPendingOrders")]
        public async Task<IActionResult> GetPendingOrders([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.FindAsync(a => a.Id == uId);
            if (vendor.IsSuspended == true)
                return BadRequest("Vendor is Suspended");
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;
            var pendingOrders = await _unitOfWork.OrderDetails.FindAllAsync(od => od.Status == SD.Status_Pending && od.VendorId == uId, skip, PageSize, new[] { "Product" });
            foreach(var order in pendingOrders)
            {
                order.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == order.ProductId, new[] { "ProductImages" });
            }
            var count = await _unitOfWork.OrderDetails.CountAsync(od => od.Status == SD.Status_Pending && od.VendorId == uId);
            return Ok(new { Count = count, PendingOrders = pendingOrders });
        }


        [HttpGet("GetOrders")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<IActionResult> GetOrders([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.FindAsync(a => a.Id == uId);
            if (vendor.IsSuspended == true)
                return BadRequest("Vendor is Suspended");
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;
            var orders = await _unitOfWork.OrderDetails.FindAllAsync(od =>  od.VendorId == uId, skip, PageSize , new[] { "Product" });
            foreach (var order in orders)
            {
                order.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == order.ProductId, new[] { "ProductImages" });
            }
            var count = await _unitOfWork.OrderDetails.CountAsync(od =>  od.VendorId == uId);
            return Ok(new { Count = count, Orders = orders });
        }
        [HttpGet("GetOrderById")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.FindAsync(a => a.Id == uId);
            if (vendor.IsSuspended == true)
                return BadRequest("Vendor is Suspended");
            var Order = await _unitOfWork.OrderDetails.FindAsync(od => od.OrderId== orderId&&od.VendorId==uId);
            if (Order == null) return NotFound();
            Order.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == Order.ProductId, new[] { "ProductImages" });
            return Ok(Order);
        }
        [HttpPut("ConfirmtPendingOrders")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<IActionResult> ConfirmtPendingOrders(int orderId , int productId)
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.FindAsync(a => a.Id == uId);
            if (vendor.IsSuspended == true)
                return BadRequest("Vendor is Suspended");

            var orderDetails =await  _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == orderId && od.ProductId == productId);
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
                    var order = await _unitOfWork.Order.FindAsync(o=> o.OrderId == orderId);
                    order.OrderStatus = SD.Status_Confirmed;
                    order.Date= DateTime.UtcNow;
                    _unitOfWork.Order.Update(order);
                    _unitOfWork.Save();
                }
            }
            return Ok();
        }
        [HttpGet("ViewOwnProducts")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<IActionResult> ViewOwnProducts([FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromQuery] string? SearchTerm)
        {
            var vendorId = _unitOfWork.getUID();
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            PageNumber ??= 1;
            PageSize ??= 10;
            var skip = (PageNumber - 1) * PageSize;
            var vendorProductsQuery = await _unitOfWork.Product.FindAllAsync(p => p.VendorId == vendorId, new[] { "ProductImages" });
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                vendorProductsQuery = vendorProductsQuery.Where(p => p.ProductName.Trim().ToLower().Contains(SearchTerm.Trim().ToLower()) || ( p.Description!=null&&p.Description.Trim().ToLower().Contains(SearchTerm.Trim().ToLower())));
            }
            var count = vendorProductsQuery.Count();
            var vendorProducts = vendorProductsQuery
                .Skip(skip.Value)
                .Take(PageSize.Value)
                .ToList();
            if (!vendorProducts.Any())
                return NoContent();
            return Ok(new { Count = count, VendorProducts = vendorProducts });
        }
        [HttpPut("AddQuantity")]
        [Authorize]
        public async Task<IActionResult> AddQuantity([FromQuery]int productId, [FromQuery]int quantity)
        {
            var vendorId = _unitOfWork.getUID();
            var product = await _unitOfWork.Product.FindAsync(u => u.ProductId == productId&&u.VendorId==vendorId);
            if (product == null)
            {
                return Unauthorized();
            }
            product.Quantity += quantity;
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();
            return Ok(product);
        }
        [HttpGet("ViewVendorProfile")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<VendorProfileDto> ViewVendorProfile()
        {
            var uId = _unitOfWork.getUID();
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);
            var VendorProfile = new VendorProfileDto()
            {
                Address = user.Address,
                FName = user.FName,
                LName = user.LName,
                UserName = user.UserName,
                Email = user.Email,
                imgUrl = user.ProfilePic,
                PhoneNumber = user.PhoneNumber,
            };
            return VendorProfile;
        }
        [HttpPut("EditVendorProfile")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<IActionResult> EditVendorProfile(VendorProfileDto vendorProfile)
        {
            var uId = _unitOfWork.getUID();
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);
            if (vendorProfile.Email == null)
            {
                return BadRequest(" email can not be null");
            }
            if (user.Email != vendorProfile.Email)
            {
                if (await _userManager.FindByEmailAsync(vendorProfile.Email) is not null)
                    return BadRequest("Email is already registered!");
            }
            user.Address = vendorProfile.Address;
            user.FName = vendorProfile.FName;
            user.LName = vendorProfile.LName;
            user.Email = vendorProfile.Email;
            user.PhoneNumber = vendorProfile.PhoneNumber;

            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Save();
            return Ok();
        }
        [HttpGet("GetSuspendedVendor")]
        [Authorize]
        public async Task<IActionResult> GetSuspendedVendor([FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromQuery] string? SearchTerm)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            PageNumber ??= 1;
            PageSize ??= 10;
            var skip = (PageNumber - 1) * PageSize;
            var vendorsQuery = await _unitOfWork.ApplicationUser.FindAllAsync(u => u.IsSuspended == true);
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                vendorsQuery = vendorsQuery.Where(p => p.UserName.Trim().ToLower().Contains(SearchTerm.Trim().ToLower()) || p.FName.Trim().ToLower().Contains(SearchTerm.Trim().ToLower()) || p.LName.Trim().ToLower().Contains(SearchTerm.Trim().ToLower()) || p.StoreName.Trim().ToLower().Contains(SearchTerm.Trim().ToLower()));
            }
            var count = vendorsQuery.Count();
            var vendors = vendorsQuery
                .Skip(skip.Value)
                .Take(PageSize.Value)
                .ToList();
            if (!vendors.Any())
            {
                return NoContent();
            }
            return Ok(new { Count = count, Vendors = vendors });
        }

    }
}
