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
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;

            var products = await _unitOfWork.Product.GetProductsAsync(reportCountThreshold);
            var count = products.Count();

            if (skip.HasValue)
            {
                products.Skip(skip.Value);
            }
            if(PageSize.HasValue)
            {
                products.Take(PageSize.Value);
            }
            if (!products.Any())
            {
                return NoContent();
            }

            return Ok(new { Count = count, Products = products });
        }

    }
}
