using Dressify.DataAccess.Repository.IRepository;
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
            var skip = (PageNumber - 1) * PageSize;

            var productReports = await _unitOfWork.ProductReport.GetAllAsync(PageSize,skip, new[] { "Product", "Customer","Vendor" });
            return Ok(productReports);
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
            var skip = (PageNumber - 1) * PageSize;

            var productReports = await _unitOfWork.ProductReport.FindAllAsync(u=>u.ReportStatus==false,skip, PageSize, new[] { "Product", "Customer", "Vendor" });
            return Ok(productReports);
        }
    }
}