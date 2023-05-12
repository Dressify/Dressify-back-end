using Dressify.DataAccess.Repository;
using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

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
        public async Task<IActionResult> NeedToPunch([FromQuery] int? PageNumber, [FromQuery] int? PageSize,[FromQuery] int? reportCountThreshold)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var skip = (PageNumber - 1) * PageSize;

            var products = await _unitOfWork.Product.GetProductsAsync(skip,PageSize,reportCountThreshold);

            if (products.Count == 0)
            {
                return NoContent();
            }

            return Ok(products);
        }

    }
}
