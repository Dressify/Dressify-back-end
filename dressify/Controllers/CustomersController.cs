using Dressify.DataAccess.Repository.IRepository;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dressify.DataAccess.Dtos;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Dressify.Models;
using System.Security.Claims;
using System.Security.Principal;
using Stripe;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomersController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("RateProduct")]
        public async Task<IActionResult> RateProduct(RateDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(_unitOfWork.getUID());
            var product = _unitOfWork.Product.GetById(obj.ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            if(obj.rate <1 || obj.rate > 5) 
            {
                return BadRequest("Rating should be between 1 to 5 ");
            }
            var result = _unitOfWork.ProductRate.Find(r => r.ProductId==obj.ProductId && r.CustomerId==user.Id);

            if (result is not null )
            {
                return BadRequest("This product has already been rated by the customer ");
            }
            else
            {
                var item = new ProductRate
                {
                    CustomerId = user.Id,
                    ProductId = obj.ProductId,
                    Rate =obj.rate,
                    RateComment = obj.RateComment,
                    //include isPurchased after adding Order 
                    Date=DateTime.Now,
                };
                await _unitOfWork.ProductRate.AddAsync(item);
                _unitOfWork.Save();

            } 
            return Ok(obj);
        }

        [Authorize]
        [HttpPost("addToWishList")]
        public async Task<IActionResult> AddToWishList(WishListDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(_unitOfWork.getUID());
            var product = await _unitOfWork.Product.GetByIdAsync(obj.ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            var item = new WishList
            {
                CustomerId = user.Id,
                ProductId = obj.ProductId,
            };
            await _unitOfWork.WishList.AddAsync(item);
            _unitOfWork.Save();
            return Ok(obj);
        }
        [Authorize]
        [HttpDelete("DeleteFromWishList")]
        public async Task<IActionResult> DeleteFromWishList([FromQuery]int ProductId)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(_unitOfWork.getUID());
            var product = _unitOfWork.Product.GetById(ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            var result = await _unitOfWork.WishList.FindAsync(w => w.ProductId == ProductId && w.CustomerId == user.Id);
            if (result == null)
                return BadRequest("This user does not have this product on Wish list");
            _unitOfWork.WishList.Delete(result);
            _unitOfWork.Save();
            return Ok(ProductId);
        }

        [Authorize]
        [HttpPost("AskQuestion")]
        public async Task<IActionResult> AskQuestion(QuestionDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(_unitOfWork.getUID());
            var product = await _unitOfWork.Product.GetByIdAsync(obj.ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            if (obj.Question == null||obj.Question=="")
            {
                return BadRequest("What's your Question?!");
            }
            var item = new ProductQuestion
            {
                Question = obj.Question,
                CustomerId = user.Id,
                ProductId = obj.ProductId,
            };
            await _unitOfWork.ProductQuestion.AddAsync(item);
            _unitOfWork.Save();
            return Ok(obj);
        }

        [HttpPost("addToCart")]
        [Authorize]
        public async Task<IActionResult> AddToCartAsync(AddToCartDto shoppingCart)
        {
            var uId = _unitOfWork.getUID();
            shoppingCart.CustomerId = uId;
            ShoppingCart cartFromDb = await _unitOfWork.ShoppingCart.FindAsync(x => x.CustomerId == uId && x.ProductId == shoppingCart.ProductId);
            var product = await _unitOfWork.Product.FindAsync(u=>u.ProductId == shoppingCart.ProductId);
            var cart = new ShoppingCart
            {
                CustomerId = shoppingCart.CustomerId,
                ProductId = shoppingCart.ProductId,
                Quantity=shoppingCart.quantity
            };
            if (product.Quantity <= 0)
                return BadRequest("Product Out of Stock");
            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(cart);
            }
            else
            {
                return BadRequest("Product already exist in Cart");
                //_unitOfWork.ShoppingCart.IncrementCount(cartFromDb, cart.quantity);
                //_unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return  Ok();
        }

        [HttpPost("ReportProduct")]
        [Authorize(Roles =SD.Role_Customer)]
        public async Task<IActionResult> ReportProduct([FromBody]CustomerReportDto report)
        {
            var uId = _unitOfWork.getUID();
            var prodcut=await _unitOfWork.Product.FindAsync(x => x.ProductId == report.ProductId);
            if(prodcut == null)
            {
                return NotFound();
            }

            var productReport = new ProductReport
            {
                CustomerId= uId,
                ProductId = report.ProductId,
                VendorId= prodcut.VendorId,
                Description= report.Description,
            };

            await _unitOfWork.ProductReport.AddAsync(productReport);
            _unitOfWork.Save();
            return Ok();
        }
        [Authorize]
        [HttpGet("ViewCustomerProfile")]
        public async Task<CustomerProfileDto> ViewCustomerProfile()
        {
            var uId  = _unitOfWork.getUID();
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id ==uId);
            var customerProfile = new CustomerProfileDto()
            {
                Address = user.Address,
                FName   = user.FName,
                LName   =user.LName,
                UserName=user.UserName,
                Email   = user.Email,
                imgUrl  = user.ProfilePic,
                PhoneNumber = user.PhoneNumber,
            };
            return customerProfile;
        }

        [Authorize]
        [HttpPut("EditCustomerProfile")]
        public async Task<IActionResult> EditCustomerProfile(CustomerEditProfileDto customerProfile)
        {
            var uId = _unitOfWork.getUID();
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);    
            if (customerProfile.Email == null)
            {
                return BadRequest(" email can not be null");
            }
            if ( user.Email != customerProfile.Email) 
            {
                if (await _userManager.FindByEmailAsync(customerProfile.Email) is not null)
                    return BadRequest("Email is already registered!");
            } 
             user.Address = customerProfile.Address;
             user.FName   = customerProfile.FName;
             user.LName   = customerProfile.LName;
             user.Email   = customerProfile.Email;
             user.PhoneNumber = customerProfile.PhoneNumber;

            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Save();
            return Ok();
        }


        [Authorize]
        [HttpDelete("DeleteCustomerCart")]
        public async Task<IActionResult> Clear()
        {
            var uId = _unitOfWork.getUID();
            var cart = await _unitOfWork.ShoppingCart.FindAllAsync(c => c.CustomerId == uId);
            if(cart == null)
                return BadRequest("Customer Don't have any products");
            _unitOfWork.ShoppingCart.DeleteRange(cart);
            _unitOfWork.Save();
            return Ok();
        }


        [Authorize]
        [HttpGet("GetCustomerOrders")]
        public async Task<IActionResult> getCusOrders([FromQuery] GetOrdersDto model)
        {
            if (model.PageNumber <= 0 || model.PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            model.PageNumber ??= 1;
            model.PageSize ??= 10;
            var uId = _unitOfWork.getUID();
            var skip = (model.PageNumber - 1) * model.PageSize;
            var customerOrders = new List<ViewOrderDto>() ;
            var result= await _unitOfWork.Order.FindAllAsync(o => o.CustomerId == uId);
            var totalCount = result.Count();
            var Orders = result
               .Skip(skip.Value)
               .Take(model.PageSize.Value)
               .ToList();
            if (!Orders.Any())
            {
                NoContent();
            }
            foreach (var order in Orders)
            {
                var orderDto = new ViewOrderDto()
                {
                    orderId = order.OrderId,
                    TotalPrice = (float)order.TotalPrice,
                    Status = order.OrderStatus,
                    paymentMethod=order.payementMethod,
                    dateTime = order.PaymentDate.Value,
                };
                var details = await _unitOfWork.OrderDetails.FindAllAsync(d => d.OrderId==order.OrderId);
                var sum = _unitOfWork.OrderDetails.OrdersQuantity(details);
                orderDto.Quantity = sum;
                customerOrders.Add(orderDto);
            }
            return Ok(new { Count = totalCount, ProductsWithAvgRates = customerOrders });
        }

        [Authorize]
        [HttpGet("getOrderDetails")]
        public async Task<IActionResult> getOrderDetails([FromQuery]int OrderId)
        {
            var uId = _unitOfWork.getUID();
            var order = await _unitOfWork.Order.FindAsync(o => o.OrderId == OrderId);
            if (order == null || order.CustomerId == uId)
                return NoContent();
            var details = await _unitOfWork.OrderDetails.FindAllAsync(d => d.OrderId ==OrderId, new[] { "Product" });
            var orderDetails = new ViewOrderDto()
            {
                orderId = order.OrderId,
                TotalPrice = (float)order.TotalPrice,
                Status = order.OrderStatus,
                paymentMethod = order.payementMethod,
                dateTime = order.PaymentDate.Value,
                ProductDetails= new List<OrderProductDetailsDto>()
            };
            foreach (var item in details)
            {
                item.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == item.ProductId, new[] { "ProductImages" });
                var productDetails = new OrderProductDetailsDto()
                {
                    img = item.Product.ProductImages.ToList()[0].ImageUrl,
                    ProductPrice = item.Price.Value,
                    Quantity = item.Quantity.Value,
                    ProductName = item.ProductName,
                };
                orderDetails.ProductDetails.Add(productDetails);
            }
            var sum = _unitOfWork.OrderDetails.OrdersQuantity(details);
            orderDetails.Quantity = sum;
            return Ok(orderDetails);
        }

    }

}
