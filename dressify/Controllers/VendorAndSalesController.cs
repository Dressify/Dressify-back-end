using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorAndSalesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public VendorAndSalesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

    //public Task<IActionResult> AddProduct(CreateProductDto dto) 
    //    { 
    //        if(!ModelState.IsValid)
    //            return BadRequest();
    //    }
    }
}
