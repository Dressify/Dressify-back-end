using CloudinaryDotNet.Actions;
using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
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
            if (result.Count() == 0)
                return BadRequest("There are no products in the Cart ");

            List<CartDto> cart = new List<CartDto>();
            foreach (var item in result)
            {
                var product = await _unitOfWork.Product.FindAsync(p => p.ProductId == item.ProductId, new[] { "ProductImages" });
                var obj = new CartDto
                {
                    Product = product,
                    IsRent  = item.IsRent,
                    quantity=item.Quantity,
                };
                cart.Add(obj);
            }
            return Ok(cart);
        }

        [HttpGet("GetOrderSummary")]
        public async Task<IActionResult> GetSummary()
        {
            var uId = _unitOfWork.getUID();
            var Summary = new SummaryDto()
            {
                ListCart = await _unitOfWork.ShoppingCart.FindAllAsync(u => u.CustomerId == uId, new[] { "Product" }),
                Order = new()
            };
            if (Summary.ListCart == null)
                return BadRequest("No Items in Customer Cart");
            Summary.Order.Customer = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);
            Summary.Order.Phone = Summary.Order.Customer.PhoneNumber;
            Summary.Order.Address = Summary.Order.Customer.Address;
            foreach (var item in Summary.ListCart)
            {
                item.Price = (double)(item.Quantity * item.Product.Price);
                Summary.Order.TotalPrice += item.Price;
            }
            return Ok(Summary);
        }

        [HttpPost("Payment")]
        public async Task<IActionResult> payment(SummaryDto Summary)
        {
            var uId = _unitOfWork.getUID();
            Summary.ListCart = await _unitOfWork.ShoppingCart.FindAllAsync(u => u.CustomerId == uId, new[] { "Product" });
            Summary.Order.CustomerId = uId;

            foreach (var cart in Summary.ListCart)
            {
               var product =await _unitOfWork.Product.FindAsync(p => p.ProductId == cart.ProductId); 
                OrderDetails orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    OrderId = Summary.Order.OrderId,
                    Quantity = cart.Quantity,
                    VendorId = product.VendorId,
                };
                _unitOfWork.OrderDetails.Add(orderDetail);
                _unitOfWork.Save();
            }
            if (Summary.Order.payementMethod == SD.PaymentMethod_Credit)
            {
                Summary.Order.payementMethod = SD.PaymentMethod_Credit;
                // Stripe
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
                {
                    Amount = (long?)(Summary.Order.TotalPrice * 100),
                    Currency = "usd",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                });
                var clientSecret = paymentIntent.ClientSecret;

                var bill = new PayBill()
                {
                    PaymentIntentId = paymentIntent.Id,
                    Status = paymentIntent.Status,
                };
                _unitOfWork.Order.Add(Summary.Order);
                _unitOfWork.PayBill.Add(bill);
                _unitOfWork.Save();
                return Ok(clientSecret);
            }
            Summary.Order.PaymentDate = DateTime.Now;
            Summary.Order.payementMethod = SD.PaymentMethod_Cash;
            Summary.Order.OrderStatus = SD.Status_Confirmed;
            _unitOfWork.Order.Add(Summary.Order);
            _unitOfWork.Save();

            return Ok();
        }


        [HttpPut("IncrementQuantity")]
        public async Task<IActionResult> Plus(int productId)
        {
            var uId = _unitOfWork.getUID();
            var cart = await _unitOfWork.ShoppingCart.FindAsync(c => c.ProductId == productId && c.CustomerId == uId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
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

        [HttpDelete("RemoveFromCart")]
        public async Task<IActionResult> Remove(int productId)
        {
            var uId = _unitOfWork.getUID();
            var cart = await _unitOfWork.ShoppingCart.FindAsync(c => c.ProductId == productId && c.CustomerId == uId);
            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Save();
            return Ok();
        }

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
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
                return Ok();
            }
            else if (order.OrderStatus == SD.Status_Pending &&  order.payementMethod == SD.PaymentMethod_Credit )
            {
                var payBill = await _unitOfWork.PayBill.FindAsync(p => p.OrderId == order.OrderId);
                if (payBill.Status == SD.PaymentStatus_Succeded)
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = payBill.PaymentIntentId,
                    };
                    var service = new RefundService();
                    Refund refund = service.Create(options);
                    order.OrderStatus = SD.Status_Cancelled;
                    _unitOfWork.Order.Update(order);
                    _unitOfWork.Save();
                    return Ok();
                }
                else return BadRequest("Customer Did not pay for Order");
            }
            else return BadRequest("Order Can not be Canceled");
        }

        //[HttpPut("OrderConfirmation")]
        //public async Task<IActionResult> OrderConfirmation(int orderId , string orderStatus)
        //{
        //    var payBill = await _unitOfWork.PayBill.FindAsync(p => p.OrderId == orderId);
        //    if (payBill == null)
        //    {
        //        return NotFound();
        //    }
        //    if(payBill.Status == null || payBill.Status == SD.PaymentStatus_Failed)
        //    {

        //    }
            
        //}

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
            var clientSecret = paymentIntent.ClientSecret;
            return Ok(clientSecret);
        }
        //[HttpPost("TestRefund")]
        //public async Task<IActionResult> TestRefund() 
        //{
        //    var options = new RefundCreateOptions
        //    {
        //        Reason = RefundReasons.RequestedByCustomer,
        //        PaymentIntent = ,
        //    };
        //    var service = new RefundService();
        //    Refund refund = service.Create(options);
        //}
    }
}
