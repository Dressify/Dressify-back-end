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
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet("GetCustomerCart")]
        public async Task<IActionResult> GetAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.UserName == claims.Value);

            var result = await _unitOfWork.ShoppingCart.FindAllAsync(C => C.CustomerId == user.Id);
            if (result.Count() == 0)
                return BadRequest("There are no products in the Cart ");

            List<CartDto> cart = new List<CartDto>();
            foreach (var item in result)
            {
                var product = await _unitOfWork.Product.FindAsync(p => p.ProductId == item.ProductId, new[] { "ProductImages" });
                CartDto obj = new CartDto
                {
                    Product = product,
                    IsRent  = item.IsRent,
                    quantity=item.quantity,
                };
                cart.Add(obj);
            }
            return Ok(cart);
        }
    }
}
