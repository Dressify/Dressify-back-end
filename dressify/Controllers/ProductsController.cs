using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetProductspage")] 
        public async Task<IActionResult> GetProductsPage(int pageNumber , int pageSize )
        {
            var skip = (pageNumber - 1) * pageSize;
            var products = await _unitOfWork.Product.FindAllAsync(u=>u.IsSuspended==false,skip,pageSize,new[] { "Vendor", "ProductImages"});
            return Ok(products);
        }


        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProduct(int id)
        {
            
            var product = await _unitOfWork.Product.FindAsync(p=> p.ProductId == id , new[] { "Vendor", "ProductImages", "Questions" }
);              if(product == null)
                    return NotFound();
            if (product.IsSuspended)
            {
                return BadRequest("product is suspended untill: " +product.SuspendedUntil);
            }
            var Details = new ProductDetailsDto
            {
                Product = product,
                quantity = 1,
            };
            return Ok(Details);
        }

        [HttpGet("GetSuspendedProducts")]
        [Authorize]
        public async Task<IActionResult> GetSuspendedProducts()
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var products = await _unitOfWork.Product.FindAllAsync(u => u.IsSuspended == true);
            return Ok(products);
        }


    }
}

