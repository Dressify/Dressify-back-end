using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository;
using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.SuperAdmin.FindAllAsync(u=>u.SuperAdminId==uId)==null)
            {
                return Unauthorized();
            }
            var result = await _unitOfWork.Admin.CreateAdminAsync(dto);
            if (result.Message != "")
                return BadRequest(result.Message);
            return Ok(result);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> SALogin([FromBody] SAdminTokenRequestDto model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var SA = _unitOfWork.SuperAdmin.Find(U => U.UserName == model.UserName);
            if (SA == null)
            {
                return NotFound();
            }
            var validatePass = new ValidatePasswordDto
            {
                Password = model.Password,
                PasswordHash = SA.PasswordHash,
                PasswordSalt = SA.PasswordSalt,
            };
            var result = _unitOfWork.SuperAdmin.ValidatePassword(validatePass);
            if (!result)
                return BadRequest("User Name or Password Wrong");
            model.ID = SA.SuperAdminId;
            var Token = new JwtSecurityTokenHandler().WriteToken(await _unitOfWork.CreateJwtToken(model));
            return Ok(Token);
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