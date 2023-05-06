﻿using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPut("CheckReport")]
        [Authorize]
        public async Task<IActionResult> CheckReport([FromHeader]int reportId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var report=await _unitOfWork.productReport.FindAsync(u => u.ReportId == reportId);
            if (report == null)
            {
                return NotFound();
            }
            report.ReportStatus = true;
            report.AdminId = uId;
            _unitOfWork.productReport.Update(report);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPut("ActionReport")]
        [Authorize]
        public async Task<IActionResult> ActionReport([FromBody] AdminReportDto reportDto)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var report = await _unitOfWork.productReport.FindAsync(u => u.ReportId == reportDto.ReportId);
            if (report == null)
            {
                return NotFound();
            }
            report.ReportStatus = true;
            report.AdminId = uId;
            report.Action = reportDto.Action;
            _unitOfWork.productReport.Update(report);
            _unitOfWork.Save();
            return Ok();
        }

        //[HttpPost("SusppendProduct")]
        //[Authorize]
        //public async Task<IActionResult> SusppendProduct(ProductActionDto actionDto)
        //{
        //    var uId = _unitOfWork.getUID();
        //    if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
        //    {
        //        return Unauthorized();
        //    }
        //    var Product=await _unitOfWork.Product.FindAsync(u=>u.ProductId == actionDto.ProductId);
        //    if (Product == null)
        //    {
        //        return NotFound(actionDto.ProductId);
        //    }
        //    Product.IsSuspended = true;
        //    Product.SuspendedUntil= actionDto.SuspendedUntil;

        //    var productAction = new ProductAction
        //    {
        //        AdminId = uId,
        //        ProductId = actionDto.ProductId,
        //        VendorId= Product.VendorId,
        //        Action = actionDto.Action,
        //        Date=DateTime.UtcNow,
        //    };

        //    _unitOfWork.Product.Update(Product);
        //    _unitOfWork.ProductAction.AddAsync(productAction);
        //    _unitOfWork.Save();
        //    return Ok(productAction);

        //}
    }
}
