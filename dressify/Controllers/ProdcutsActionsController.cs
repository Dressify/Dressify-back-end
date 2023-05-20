using Dressify.DataAccess.Repository;
using Dressify.DataAccess.Repository.IRepository;
using MailKit.Search;
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
        public async Task<IActionResult> NeedToPunch([FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromQuery] string? SearchTerm, [FromQuery] int? reportCountThreshold)
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
            PageNumber ??= 1;
            PageSize ??= 10;
            var skip = (PageNumber - 1) * PageSize;

            var productsQuery = _unitOfWork.Product.GetProducts(reportCountThreshold);

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                productsQuery = productsQuery.Where(p=>p.ProductName.Trim().ToLower().Contains(SearchTerm.Trim().ToLower()) ||(p.Description!=null && p.Description.Trim().ToLower().Contains(SearchTerm.Trim().ToLower())));
            }

            var count = productsQuery.Count();

            var products = productsQuery
                .Skip(skip.Value)
                .Take(PageSize.Value)
                .ToList();

            return Ok(new { Count = count, Products = products });
        }

    }
}
