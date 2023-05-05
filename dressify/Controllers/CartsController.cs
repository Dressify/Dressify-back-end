using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
