using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
        public async Task<IActionResult> CheckReport([FromQuery]int reportId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var report=await _unitOfWork.ProductReport.FindAsync(u => u.ReportId == reportId);
            if (report == null)
            {
                return NotFound();
            }
            report.ReportStatus = true;
            report.AdminId = uId;
            _unitOfWork.ProductReport.Update(report);
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
            RecurringJob.AddOrUpdate(() => _unitOfWork.Unsuspend(), Cron.Daily(0));

            var report = await _unitOfWork.ProductReport.FindAsync(u => u.ReportId == reportDto.ReportId);
            if (report == null)
            {
                return NotFound();
            }
            report.ReportStatus = true;
            report.AdminId = uId;
            report.Action = reportDto.Action;
            _unitOfWork.ProductReport.Update(report);
            _unitOfWork.Save();

            if (reportDto.Action == SD.Action_SuspendProduct)
            {
                var productAction = new ProductActionDto()
                {
                    ProductId = report.ProductId,
                    Reasson= "because report ID : "+report.ReportId.ToString(),
                    SuspendedUntil=reportDto.SuspendedUntil,
                };
                return await SuspendProduct(productAction);
            }
            if (reportDto.Action == SD.Action_SuspendVendor)
            {
                var vendorAction = new VendorActionDto()
                {
                    VendorId = report.VendorId,
                    Reasson = "because report ID : " + report.ReportId.ToString(),
                    SuspendedUntil = reportDto.SuspendedUntil,
                };
                return await SuspendVendor(vendorAction);
            }     
            return Ok(report);
        }

        [HttpPut("SuspendProduct")]
        [Authorize]
        public async Task<IActionResult> SuspendProduct(ProductActionDto actionDto)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var Product = await _unitOfWork.Product.FindAsync(u => u.ProductId == actionDto.ProductId);
            if (Product == null)
            {
                return NotFound(actionDto.ProductId);
            }
            Product.IsSuspended = true;
            if (actionDto.SuspendedUntil == null)
            {
                Product.SuspendedUntil = DateTime.UtcNow.AddYears(100);
            }
            else
            {
                DateTime suspendedUntil;
                if (!DateTime.TryParseExact(actionDto.SuspendedUntil, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out suspendedUntil))
                {
                    return BadRequest("Invalid date format. Please use yyyy-MM-dd format.");
                }
                Product.SuspendedUntil = suspendedUntil;
            }
            var productAction = new ProductAction
            {   
                AdminId = uId,
                ProductId = actionDto.ProductId,
                VendorId = Product.VendorId,
                Reasson = actionDto.Reasson,
                Date = DateTime.UtcNow,
            };

            _unitOfWork.Product.Update(Product);
            _unitOfWork.ProductAction.AddAsync(productAction);
            _unitOfWork.Save();
            return Ok(productAction);

        }

        [HttpPut("UnSuspenedProduct")]
        [Authorize]
        public async Task<IActionResult> UnSuspenedProduct([FromQuery] int productId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var Product = await _unitOfWork.Product.FindAsync(u => u.ProductId == productId);
            if (Product == null)
            {
                return NotFound(productId);
            }
            Product.IsSuspended = false;
            Product.SuspendedUntil = null;
            _unitOfWork.Product.Update(Product);
            _unitOfWork.Save();
            return Ok(Product);
        }

        [HttpPut("SuspendVendor")]
        [Authorize]
        public async Task<IActionResult> SuspendVendor(VendorActionDto actionDto)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var vendor = await _unitOfWork.ApplicationUser.GetUserAsync(actionDto.VendorId);
            if (vendor == null)
            {
                return NotFound(actionDto.VendorId);
            }
            vendor.IsSuspended = true;
            if (actionDto.SuspendedUntil == null)
            {
                vendor.SuspendedUntil = DateTime.UtcNow.AddYears(100);
            }
            else
            {
                DateTime suspendedUntil;
                if (!DateTime.TryParseExact(actionDto.SuspendedUntil, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out suspendedUntil))
                {
                    return BadRequest("Invalid date format. Please use yyyy-MM-dd format.");
                }
                vendor.SuspendedUntil = suspendedUntil;
            }
            var Penalty = new Penalty
            {
                AdminId = uId,
                VendorId = actionDto.VendorId,
                Reasson = actionDto.Reasson,
                Date = DateTime.UtcNow,
            };

            var products = await _unitOfWork.Product.FindAllAsync(u => u.VendorId == actionDto.VendorId);
            if (products.Any())
            {
                foreach (var product in products)
                {
                    if (product.IsSuspended == false)
                    {
                        product.IsSuspended = true;
                        product.SuspendedUntil = vendor.SuspendedUntil;
                        _unitOfWork.Product.Update(product);
                    }
                    else if (product.SuspendedUntil <= vendor.SuspendedUntil)
                    {
                        product.IsSuspended = true;
                        product.SuspendedUntil = vendor.SuspendedUntil;
                        _unitOfWork.Product.Update(product);
                    }
                }
            }
            _unitOfWork.ApplicationUser.Update(vendor);
            await _unitOfWork.Penalty.AddAsync(Penalty);
            _unitOfWork.Save();
            return Ok(Penalty);
        }

        [HttpPut("UnSuspenedVendor")]
        [Authorize]
        public async Task<IActionResult> UnSuspenedVendor([FromQuery] string VendorId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var Vendor = await _unitOfWork.ApplicationUser.GetUserAsync(VendorId);
            if (Vendor == null)
            {
                return NotFound(VendorId);
            }
            if(Vendor.SuspendedUntil>= DateTime.UtcNow.AddYears(90))
            {
                var products = await _unitOfWork.Product.FindAllAsync(u => u.VendorId == VendorId);
                if (products.Any())
                {
                    foreach (var product in products)
                    {
                        product.IsSuspended = false;
                        product.SuspendedUntil = null;
                        _unitOfWork.Product.Update(product);
                    }
                }
            }
            Vendor.IsSuspended = false;
            Vendor.SuspendedUntil = null;
            _unitOfWork.ApplicationUser.Update(Vendor);
            _unitOfWork.Save();
            return Ok(Vendor);
        }

    }
}
