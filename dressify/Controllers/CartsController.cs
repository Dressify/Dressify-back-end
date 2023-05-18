using CloudinaryDotNet.Actions;
using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet("GetCustomerCart")]
        public async Task<IActionResult> GetAsync()
        {
            var uId = _unitOfWork.getUID();
            var result = await _unitOfWork.ShoppingCart.FindAllAsync(C => C.CustomerId == uId);
            var count = await _unitOfWork.ShoppingCart.CountAsync();
            if (!result.Any())
                return BadRequest("There are no products in the Cart ");
            List<CartDto> cart = new List<CartDto>();
            foreach (var item in result)
            {
                var product = await _unitOfWork.Product.FindAsync(p => p.ProductId == item.ProductId, new[] { "ProductImages" });
                if (product.IsSuspended)
                {
                    _unitOfWork.ShoppingCart.Delete(item);
                    _unitOfWork.Save();
                }
                else
                {
                    var obj = new CartDto
                    {
                        Product = product,
                        quantity = item.Quantity,
                        price = _unitOfWork.CalculatePrice(item.Quantity.Value, product.Price,product.Sale)
                    };
                    
                    cart.Add(obj);
                };
            }
            if (!cart.Any())
            {
                return BadRequest("There are no products in the Cart ");
            }
            return Ok(new { Count = count, Carts = cart });
        }

        [HttpGet("GetOrderSummary")]
        public async Task<IActionResult> GetSummary()
        {
            var uId = _unitOfWork.getUID();
            var ListCart = await _unitOfWork.ShoppingCart.FindAllAsync(u => u.CustomerId == uId, new[] { "Product" });

            if (ListCart == null)
                return BadRequest("No Items in Customer Cart");
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);

            var summary = new SummaryDto()
            {
                Phone = user.PhoneNumber,
                Fname = user.FName,
                Lname = user.LName,
                Address = user.Address,
                Email = user.Email,
                detailsList = new List<SummaryDetailsListDto>()
        };
            foreach (var item in ListCart)
            {
                var productDetail = new SummaryDetailsListDto()
                {
                    Quantity = item.Quantity.Value,
                    ProductName = item.Product.ProductName,
                    Price = _unitOfWork.CalculatePrice(item.Quantity.Value, item.Product.Price, item.Product.Sale),
                };
               summary.detailsList.Add(productDetail);
            }
            return Ok(summary);
        }
        [HttpPost("paywithCash")]
        public async Task<IActionResult> Cash(PayDto dto)
        {
            var uId = _unitOfWork.getUID();
            var cartList = await _unitOfWork.ShoppingCart.FindAllAsync(u => u.CustomerId == uId, new[] { "Product" });
            var order = new Order();
            order.PaymentDate = DateTime.Now;
            order.Address = dto.Address;
            order.Phone = dto.Phone;
            order.OrderStatus = SD.Status_Pending;
            order.payementMethod = SD.PaymentMethod_Cash;
            order.CustomerId = uId;
            order.TotalPrice = 0;
            _unitOfWork.Order.Add(order);
            _unitOfWork.Save();
            foreach (var cart in cartList)
            {
                var product = await _unitOfWork.Product.FindAsync(p => p.ProductId == cart.ProductId);
                OrderDetails orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    Price = (double?)_unitOfWork.CalculatePrice(1, product.Price, product.Sale),
                    OrderId = order.OrderId,
                    Quantity = cart.Quantity,
                    VendorId = product.VendorId,
                    Status =SD.Status_Pending,
                    ProductName = product.ProductName,
                };
                order.TotalPrice += (orderDetail.Price * orderDetail.Quantity);
                product.Quantity -= cart.Quantity.Value;
                product.Purchases += cart.Quantity.Value;
                _unitOfWork.OrderDetails.Add(orderDetail);
                _unitOfWork.Product.Update(product);
            }
            _unitOfWork.ShoppingCart.DeleteRange(cartList);
            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPost("paywithCredit")]
        public async Task<IActionResult> PayWithStripe(PayDto dto)
        {
            var uId = _unitOfWork.getUID();
            var cartList = await _unitOfWork.ShoppingCart.FindAllAsync(u => u.CustomerId == uId, new[] { "Product" });
            var order = new Order();
            order.payementMethod = SD.PaymentMethod_Credit;
            order.CustomerId = uId;
            order.PaymentDate = DateTime.Now;
            order.Address = dto.Address;
            order.Phone = dto.Phone;
            order.OrderStatus = SD.Status_Pending;
            order.TotalPrice = 0;
            _unitOfWork.Order.Add(order);
            _unitOfWork.Save();
            foreach (var cart in cartList)
            {
                var product = await _unitOfWork.Product.FindAsync(p => p.ProductId == cart.ProductId);
                OrderDetails orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    Price = (double?)_unitOfWork.CalculatePrice(1, product.Price, product.Sale),
                    OrderId = order.OrderId,
                    Quantity = cart.Quantity,
                    VendorId = product.VendorId,
                    ProductName = product.ProductName,
                    Status =SD.Status_Pending
                };
                order.TotalPrice += (orderDetail.Price * orderDetail.Quantity);
                _unitOfWork.OrderDetails.Add(orderDetail);
                product.Purchases += cart.Quantity.Value;
                product.Quantity -= cart.Quantity.Value;
                _unitOfWork.OrderDetails.Add(orderDetail);
                _unitOfWork.Product.Update(product);
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
            }
            var payment = await _unitOfWork.createPaymentIntent(order.OrderId);
            var clientSecret = payment.ClientSecret;

            var bill = new PayBill()
            {
                PaymentIntentId = payment.paymentIntentId,
                Status = SD.PaymentStatus_Succeded,
                OrderId= order.OrderId,
            };           
            _unitOfWork.PayBill.Add(bill);
            _unitOfWork.ShoppingCart.DeleteRange(cartList);
            _unitOfWork.Save();
            return Ok(clientSecret);
        }

        [HttpPut("IncrementQuantity")]
        public async Task<IActionResult> Plus(int productId)
        {
            var uId = _unitOfWork.getUID();
            var product = await _unitOfWork.Product.FindAsync(u => u.ProductId == productId);
            var cart = await _unitOfWork.ShoppingCart.FindAsync(c => c.ProductId == productId && c.CustomerId == uId);
            var newQuantity= _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            if (newQuantity > product.Quantity)
                return BadRequest("There is not enough product");
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPut("DecrementQuantity")]
        public async Task<IActionResult> Minus(int productId)
        {
            var uId = _unitOfWork.getUID();
            var cart = await _unitOfWork.ShoppingCart.FindAsync(c => c.ProductId == productId && c.CustomerId == uId);
            if (cart.Quantity <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);

            }
            _unitOfWork.Save();
            return Ok();
        }
        [Authorize]
        [HttpDelete("RemoveFromCart")]
        public async Task<IActionResult> Remove(int productId)
        {
            var uId = _unitOfWork.getUID();
            var cart = await _unitOfWork.ShoppingCart.FindAsync(c => c.ProductId == productId && c.CustomerId == uId);
            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Save();
            return Ok();
        }
        [Authorize]
        [HttpPut("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var uId = _unitOfWork.getUID();
            var  order = _unitOfWork.Order.GetById(orderId);
            if (order == null)
                return BadRequest("Order Does not exist");
            if (order.OrderStatus == SD.Status_Pending && order.payementMethod == SD.PaymentMethod_Cash  )
            {
                order.OrderStatus = SD.Status_Cancelled;
                order.Date = DateTime.UtcNow;
                _unitOfWork.OrderDetails.returnProductQuantity(order.OrderId);
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
                return Ok();
            }
            else if (order.OrderStatus == SD.Status_Pending &&  order.payementMethod == SD.PaymentMethod_Credit )
            {
                var payBill = await _unitOfWork.PayBill.FindAsync(p => p.OrderId == order.OrderId);
                if (payBill.Status == SD.PaymentStatus_Succeded)
                {
                    _unitOfWork.refund(payBill.PaymentIntentId);
                    _unitOfWork.OrderDetails.returnProductQuantity(order.OrderId);
                    order.OrderStatus = SD.Status_Cancelled;
                    order.Date = DateTime.UtcNow;
                    _unitOfWork.Order.Update(order);
                    _unitOfWork.Save();
                    return Ok();
                }
                else return BadRequest("Customer Did not pay for Order");
            }
            else return BadRequest("Order Can not be Canceled");
        }

        [Authorize]
        [HttpPost("testPay")]
        public async Task<IActionResult> TestPay()
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = (long?)(50000 * 100),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });
            var Status = paymentIntent.Status;

            var clientSecret = paymentIntent.ClientSecret;
            return Ok(clientSecret);
        }
    }
}
