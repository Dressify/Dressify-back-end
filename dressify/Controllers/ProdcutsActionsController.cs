using Dressify.DataAccess.Repository;
using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdcutsActionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProdcutsActionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //Product has more than 10 reports that have been checked
        [HttpGet("NeedToPunch")]
        [Authorize]
        public async Task<IActionResult> NeedToPunch([FromQuery] int? reportCountThreshold = 10)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }

            var products = await _unitOfWork.Product.GetProductsAsync(reportCountThreshold.Value);

            if (products.Count == 0)
            {
                return NoContent();
            }

            return Ok(products);
        }

    }
}
