using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("GetAllReports")]
        [Authorize]
        public async Task<IActionResult> GetAllReports([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
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

            var productReports = await _unitOfWork.ProductReport.GetAllAsync(PageSize,skip, new[] { "Product", "Customer","Vendor" });
            foreach (var productReport in productReports)
            {
                productReport.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == productReport.ProductId, new[] { "ProductImages" });
            }
            var count = await _unitOfWork.ProductReport.CountAsync();

            return Ok(new { Count = count, ProductReports = productReports });
        }

        [HttpGet("GetUncheckedReports")]
        [Authorize]
        public async Task<IActionResult> GetUncheckedReports([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
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

            var productReports = await _unitOfWork.ProductReport.FindAllAsync(u=>u.ReportStatus==false,skip, PageSize, new[] { "Product", "Customer", "Vendor" });
            foreach (var productReport in productReports)
            {
                productReport.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == productReport.ProductId, new[] { "ProductImages" });
            }
            var count = await _unitOfWork.ProductReport.CountAsync();

            return Ok(new { Count = count, ProductReports = productReports });
        }

        [HttpGet("GetReportByID")]
        [Authorize]
        public async Task<IActionResult> GetReportByID([FromQuery]int reportId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }

            var productReport = await _unitOfWork.ProductReport.FindAsync(u => u.ReportId==reportId, new[] { "Product", "Customer", "Vendor" });
                productReport.Product = await _unitOfWork.Product.FindAsync(p => p.ProductId == productReport.ProductId, new[] { "ProductImages" });

            return Ok(productReport);
        }
    }
}