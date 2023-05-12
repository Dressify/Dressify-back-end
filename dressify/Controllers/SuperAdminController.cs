﻿using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> CreateAdmin(IFormFile Photo, [FromForm]AddAdminDto dto)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.SuperAdmin.FindAllAsync(u => u.SuperAdminId == uId) == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _unitOfWork.Admin.FindAsync(u => u.Email == dto.Email) is not null)
                return BadRequest("Email is already registered");

            if (await _unitOfWork.Admin.FindAsync(u => u.AdminName == dto.AdminName) is not null)
                return BadRequest("Username is already registered!");

            CreatePhotoDto photodto = await _unitOfWork.Admin.AddPhoto(Photo);
            dto.PublicId = photodto.PublicId;
            dto.ProfilePic = photodto.Url;

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
            if (adminId == null)
            {
                return BadRequest("Enter an AdminID");
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
            if(adminPorfile.AdminId == null)
            {
                return BadRequest("Enter an AdminID");
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
            if (admin.AdminName != adminPorfile.AdminName)
            {
                if (await _unitOfWork.Admin.FindAsync(u => u.AdminName == adminPorfile.AdminName) != null)
                    return BadRequest("Admin Name is already registered!");
            }
            admin.AdminName = adminPorfile.AdminName;
            admin.Email = adminPorfile.Email;
            _unitOfWork.Admin.Update(admin);
            _unitOfWork.Save();
            return Ok(adminPorfile);
        }

        [HttpPut("ModifyAdminPhoto")]
        public async Task<IActionResult> ModifyAdminPhoto(IFormFile photo , [FromHeader] string adminId)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.SuperAdmin.FindAllAsync(u => u.SuperAdminId == uId) == null)
            {
                return Unauthorized();
            }
            var admin = await _unitOfWork.Admin.FindAsync(u => u.AdminId == adminId);
            if (admin != null)
            {
                if (admin.ProfilePic != null)
                {
                    var res = await _unitOfWork.Admin.DeletePhoto(admin.PublicId);
                }
                CreatePhotoDto result = await _unitOfWork.Admin.AddPhoto(photo);
                admin.PublicId = result.PublicId;
                admin.ProfilePic = result.Url;
                _unitOfWork.Admin.Update(admin);
                return Ok(result);
            }
            return NotFound();
        }

    }
}