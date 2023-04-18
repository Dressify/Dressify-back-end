using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("listAllProducts")] 
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _unitOfWork.Product.GetAllAsync(new[] { "Vendor", "ProductImages", "Questions" });
            return Ok(products);
        }
        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProduct(int id)
        {
            
            var product = await _unitOfWork.Product.FindAsync(p=> p.ProductId == id , new[] { "Vendor", "ProductImages", "Questions" }
);              if(product == null)
                    return NotFound();
            return Ok(product);
        }
    }
}

