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


        

        [Authorize]
        [HttpGet("TestSUPerAdmin")]
        public async Task<IActionResult> Test()
        {
            var uId = _unitOfWork.getUID();
            return Ok("niceeeeeeeeee");
        }
    }
}