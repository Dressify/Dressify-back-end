using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("ViewAdminProfile")]
        [Authorize]
        public async Task<IActionResult> ViewAdminProfile()
        {
            var Uid = _unitOfWork.getUID();
            var admin = await _unitOfWork.Admin.FindAsync(u => u.AdminId == Uid);
            if (admin==null)
            {
                return Unauthorized();
            }
            var adminProfile = new AdminPorfileDto()
            {
                AdminId = admin.AdminId,
                AdminName = admin.AdminName,
                ProfilePic=admin.ProfilePic,
                Email=admin.Email,
            };
            return Ok(adminProfile);
        }

        [HttpPost("CreateSales")]
        [Authorize]
        public async Task<IActionResult> CreateSales([FromForm] AddSalesDto dto)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAllAsync(u => u.AdminId == uId) == null)
                return Unauthorized();
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                return BadRequest("Email is already registered!");

            if (await _userManager.FindByNameAsync(dto.SalesName) is not null)
                return BadRequest("Username is already registered!");
            var user = new ApplicationUser
            {
                UserName = dto.SalesName,
                Email = dto.Email,
                NId = dto.NId,
                FName = dto.FName,
                LName = dto.LName,
                PhoneNumber = SD.Phone,
                Address = SD.Address,
                ProfilePic=SD.ImgUrl,
                PublicId=SD.PublicId,
                StoreName=SD.StoreName,

            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return BadRequest(errors);
            }
            await _userManager.AddToRoleAsync(user, SD.Role_Sales);
            await _userManager.UpdateAsync(user);
            _unitOfWork.Save();
            return Ok(new
            {
                UserName = user.UserName,
                Email = user.Email,
                NId = user.NId,
                FName = user.FName,
                LName = user.LName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ProfilePic = user.ProfilePic,
                PublicId = user.PublicId,
                StoreName = user.StoreName,
                Role = SD.Role_Sales,
            });
        }

        [HttpGet("GetAllSales")]
        [Authorize]
        public async Task<IActionResult> GetAllSales([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAllAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }

            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }

            var skip = (PageNumber - 1) * PageSize;

            var sales = await _unitOfWork.ApplicationUser.FindAllAsync(u=>u.StoreName==SD.StoreName,PageSize, skip);
            var count = await _unitOfWork.ApplicationUser.CountAsync(u => u.StoreName == SD.StoreName);
            if (!sales.Any())
            {
                return NoContent();
            }

            var SalesDtos = sales.Select(sales => new AllSalesDto
            {
                SalesId = sales.Id,
                SalesName = sales.UserName,
                Email = sales.Email,
                ProfilePic = sales.ProfilePic
            }).ToList();
            return Ok(new { Count = count, Sales = SalesDtos });
        }

        [HttpGet("GetAllVendors")]
        [Authorize]
        public async Task<IActionResult> GetAllVendors([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAllAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }

            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }

            var skip = (PageNumber - 1) * PageSize;

            var vendors = await _unitOfWork.ApplicationUser.FindAllAsync(u => u.StoreName != SD.StoreName, PageSize, skip);
            var count = await _unitOfWork.ApplicationUser.CountAsync(u => u.StoreName != SD.StoreName);
            if (!vendors.Any())
            {
                return NoContent();
            }

            var vendorsDtos = vendors.Select(sales => new AllSalesDto
            {
                SalesId = sales.Id,
                SalesName = sales.UserName,
                Email = sales.Email,
                ProfilePic = sales.ProfilePic
            }).ToList();
            return Ok(new { Count = count, Vendors = vendorsDtos });
        }

        [HttpGet("GetSalesProfile")]
        [Authorize]
        public async Task<IActionResult> GetSalesProfile([FromHeader] string SalesId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAllAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            if (SalesId == null)
            {
                return BadRequest("Enter an SalesId");
            }
            var sales = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == SalesId);
            if (sales == null)
            {
                return NoContent();
            }
            var salesProfile = new SalesProfileDto()
            {
                SalesId = SalesId,
                SalesName = sales.UserName,
                ProfilePic = sales.ProfilePic,
                Email = sales.Email,
                Phone=sales.PhoneNumber,
                FName = sales.FName,
                LName = sales.LName,
                Address=sales.Address,
                NId=sales.NId,
                StoreName=sales.StoreName
            };
            return Ok(salesProfile);
        }

        [HttpPut("EditSalesProfile")]
        [Authorize]
        public async Task<IActionResult> EditSalesProfile(EditSalesProfileDto dto)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAllAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (dto.SalesId == null)
            {
                return BadRequest("Enter an SalesId");
            }
            var sales = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == dto.SalesId);
            if (sales == null)
            {
                return NotFound();
            }
            if (sales.Email != dto.Email)
            {
                if (await _unitOfWork.ApplicationUser.FindAsync(u => u.Email == dto.Email) != null)
                    return BadRequest("Email is already registered!");
            }
            if (sales.UserName != dto.SalesName)
            {
                if (await _unitOfWork.ApplicationUser.FindAsync(u => u.UserName == dto.SalesName) != null)
                    return BadRequest("Admin Name is already registered!");
            }
            sales.UserName = dto.SalesName;
            sales.Email = dto.Email;
            sales.FName = dto.FName;
            sales.LName=dto.LName;
            sales.NId = dto.NId;

            _unitOfWork.ApplicationUser.Update(sales);
            _unitOfWork.Save();
            return Ok(dto);
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
