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
        public async Task<IActionResult> GetAsync()
        {
            var result = await _unitOfWork.WishList.FindAllAsync(C=> C.CustomerId == _unitOfWork.getUID());
            if(result.Count()==0)
                return Ok("There are no products in the whish list ");

            List<Product> productList = new List<Product>();
            foreach (var item in result)
            {
               var product= await _unitOfWork.Product.FindAsync(p => p.ProductId == item.ProductId&& p.IsSuspended==false, new[] { "ProductImages" });
                productList.Add(product);
            }
            return Ok(productList);
        }
        

    }
}
