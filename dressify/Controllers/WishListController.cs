using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public WishListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("GetCustomerWishList")]
        public async Task<IActionResult> GetAsync(string customerId)
        {
            var result = await _unitOfWork.WishList.FindAllAsync(C=> C.CustomerId == customerId);
            if(result.Count()==0)
                return Ok("There are no products in the whish list ");

            List<Product> productList = new List<Product>();
            foreach (var item in result)
            {
               var product= await _unitOfWork.Product.FindAsync(p => p.ProductId == item.ProductId, new[] { "ProductImages" });
                productList.Add(product);
            }
            return Ok(productList);
        }
        [HttpPost("addToWishList")]
        public async Task<IActionResult> AddToWishList(WishListDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(obj.CustomerId);
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
                CustomerId = obj.CustomerId,
                ProductId = obj.ProductId,
            };
            await _unitOfWork.WishList.AddAsync(item);
            _unitOfWork.Save();
            return Ok(obj);
        }

        [HttpDelete("DeleteFromWishList")]
        public async Task<IActionResult> DeleteFromWishList(WishListDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(obj.CustomerId);
            var product = _unitOfWork.Product.GetById(obj.ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            var result = await _unitOfWork.WishList.FindAsync(w => w.ProductId == obj.ProductId && w.CustomerId == obj.CustomerId);
            if (result == null)
                return BadRequest("This user does not have this product on Wish list");
            _unitOfWork.WishList.Delete(result);
            _unitOfWork.Save();
            return Ok(obj);
        }

    }
}
