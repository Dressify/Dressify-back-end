using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Utility;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace dressify.Controllers
{
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.ApplicationUser.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.ApplicationUser.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            return Ok(result);
        }


        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminTokenRequestDto model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            //if he is Admin
            var admin = _unitOfWork.Admin.Find(U => U.AdminName == model.UserName);
            if (admin !=null)
            {
                var validatePass = new ValidatePasswordDto
                {
                    Password = model.Password,
                    PasswordHash = admin.PasswordHash,
                    PasswordSalt = admin.PasswordSalt,
                };
                var validatePassword = _unitOfWork.SuperAdmin.ValidatePassword(validatePass);
                if (!validatePassword)
                    return BadRequest("User Name or Password Wrong");
                model.ID = admin.AdminId;
                var result = await _unitOfWork.CreateJwtToken(model);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);
                result.Message = "Authenticated";
                result.Email=admin.Email;
                result.Role = SD.Role_Admin;
                result.ImgUrl = admin.ProfilePic;
                return Ok(result);
            }

            //if he is SuperAdmin
            var SAdmin = _unitOfWork.SuperAdmin.Find(U => U.UserName == model.UserName);
            if (SAdmin != null)
            {
                var validatePass = new ValidatePasswordDto
                {
                    Password = model.Password,
                    PasswordHash = SAdmin.PasswordHash,
                    PasswordSalt = SAdmin.PasswordSalt,
                };
                var validatePassword = _unitOfWork.SuperAdmin.ValidatePassword(validatePass);
                if (!validatePassword)
                    return BadRequest("User Name or Password Wrong");
                model.ID = SAdmin.SuperAdminId;
                var result = await _unitOfWork.CreateJwtToken(model);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);
                result.Message = "Authenticated";
                result.Role =SD.Role_SuperAdmin;
                return Ok(result);
            }
            //If he is neither than admin nor than super admin
            return BadRequest("User Name or Password Wrong");
        }


    }
}
