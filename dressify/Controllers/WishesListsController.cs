using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishesListsController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public WishesListsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("GetCustomerWishList")]
        public async Task<IActionResult> GetAsync([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var result = await _unitOfWork.WishList.FindAllAsync(C=> C.CustomerId == _unitOfWork.getUID());
            if(!result.Any())
                return Ok("There are no products in the whish list ");
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }

            var skip = (PageNumber - 1) * PageSize;
            List<Product> productList = new List<Product>();
            foreach (var item in result)
            {
               var product= await _unitOfWork.Product.FindAsync(p => p.ProductId == item.ProductId&& p.IsSuspended==false, new[] { "ProductImages" });
                productList.Add(product);
            }
            var count = productList.Count();

            if (skip.HasValue)
            {
                productList.Skip(skip.Value);
            }
            if (PageSize.HasValue)
            {
                productList.Take(PageSize.Value);
            }
            return Ok(new { Count = count, ProductList = productList });
        }


    }
}
