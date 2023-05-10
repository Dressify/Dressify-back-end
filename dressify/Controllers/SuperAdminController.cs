using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController : ControllerBase
    {
 
        private readonly IUnitOfWork _unitOfWork;
        public SuperAdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("CreateAdmin")]
        [Authorize]
        public async Task<IActionResult> CreateAdmin([FromBody] AddAdminDto dto)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.SuperAdmin.FindAllAsync(u=>u.SuperAdminId==uId)==null)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _unitOfWork.Admin.CreateAdminAsync(dto);
            if (result.Message != "")
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet("GetAllAdmins")]
        [Authorize]
        public async Task<IActionResult> GetAllAdmins()
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.SuperAdmin.FindAllAsync(u => u.SuperAdminId == uId) == null)
            {
                return Unauthorized();
            }
            var admins = await _unitOfWork.Admin.GetAllAsync();

            if (!admins.Any())
            {
                return NoContent();
            }

            var adminsDtos = admins.Select(admin => new AllAdminsDto
            {
                AdminId = admin.AdminId,
                AdminName = admin.AdminName,
                Email = admin.Email,
                ProfilePic = admin.ProfilePic
            }).ToList();
            return Ok(adminsDtos);
        }

        [HttpGet("GetAdminProfile")]
        [Authorize]
        public async Task<IActionResult> GetAdminProfile([FromHeader]string adminId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.SuperAdmin.FindAllAsync(u => u.SuperAdminId == uId) == null)
            {
                return Unauthorized();
            }
            var admin = await _unitOfWork.Admin.FindAsync(u => u.AdminId == adminId);
            if (admin == null)
            {
                return NoContent();
            }
            var adminProfile = new AdminPorfileDto()
            {
                AdminName = admin.AdminName,
                ProfilePic = admin.ProfilePic,
                Email = admin.Email,
            };
            return Ok(adminProfile);
        }

        [HttpPut("EditAdminProfile")]
        [Authorize]
        public async Task<IActionResult> EditAdminProfile(AdminPorfileDto adminPorfile)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.SuperAdmin.FindAllAsync(u => u.SuperAdminId == uId) == null)
            {
                return Unauthorized();
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var admin = await _unitOfWork.Admin.FindAsync(u => u.AdminId == adminPorfile.AdminId);
            if (admin == null)
            {
                return NotFound();
            }
            if (admin.Email != adminPorfile.Email)
            {
                if (await _unitOfWork.Admin.FindAsync(u=>u.Email== adminPorfile.Email) != null)
                    return BadRequest("Email is already registered!");
            }
            admin.AdminName = adminPorfile.AdminName;
            admin.ProfilePic = adminPorfile.ProfilePic;
            admin.Email = adminPorfile.Email;
            _unitOfWork.Admin.Update(admin);
            _unitOfWork.Save();
            return Ok(adminPorfile);
        }

        [Authorize]
        [HttpGet("TestSUPerAdmin")]
        public async Task<IActionResult> Test()
        {
            var uId = _unitOfWork.getUID();
            return Ok("niceeeeeeeeee");
        }
    }
}