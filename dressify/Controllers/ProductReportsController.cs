﻿using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("GetAllReports")]
        [Authorize]
        public async Task<IActionResult> GetAllReports()
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var productReports = await _unitOfWork.productReport.GetAllAsync(new[] { "Product" });
            return Ok(productReports);
        }

        [HttpGet("GetUncheckedReports")]
        [Authorize]
        public async Task<IActionResult> GetUncheckedReports()
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var productReports = await _unitOfWork.productReport.FindAllAsync(u=>u.ReportStatus==false,new[] {  "Product" });
            return Ok(productReports);
        }
    }
}