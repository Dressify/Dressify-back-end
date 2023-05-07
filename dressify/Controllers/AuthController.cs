using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
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
        [HttpPost("CustRegister")]
        public async Task<IActionResult> CustRegisterAsync([FromBody] CustRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.ApplicationUser.CustomerRegisterAsync(model);
            if(!result.IsAuthenticated)
                return BadRequest(result.Message);
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

        //Modify Photo For Vendor , Customer and Admin
        [HttpPut("ModifyPhoto")]
        public async Task<IActionResult> modifyPhoto(IFormFile photo)
        {
            var uId = _unitOfWork.getUID();
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);
            if (user != null) 
            {
                if (user.ProfilePic != null)
                {
                    var res = await _unitOfWork.ApplicationUser.DeletePhoto(user.PublicId);
                    if (res == "ok")
                    {
                        user.ProfilePic = null;
                        user.PublicId = null;
                    }
                }
                CreatePhotoDto result = await _unitOfWork.ApplicationUser.AddPhoto(photo);
                user.PublicId = result.PublicId;
                user.ProfilePic = result.Url;
                _unitOfWork.Save();
                return Ok(result.Url);
            }
            var admin = await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId);
            if (admin != null)
            {
                if (admin.ProfilePic != null)
                {
                    var res = await _unitOfWork.Admin.DeletePhoto(admin.PublicId);
                    if (res == "ok")
                    {
                        admin.ProfilePic = null;
                        admin.PublicId = null;
                    }
                }
                CreatePhotoDto result = await _unitOfWork.Admin.AddPhoto(photo);
                admin.PublicId = result.PublicId;
                admin.ProfilePic = result.Url;
                //_unitOfWork.Admin.Update(admin);
                return Ok(result.Url);
            }
        return Unauthorized();
        }

        //Delete Photo For Vendor , Customer and Admin
        [HttpDelete("DeletePhoto")]
        public async Task<IActionResult> DelePhoto()
        {
            var uId = _unitOfWork.getUID();
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Id == uId);
            if (user != null)
            {
                if (user.ProfilePic != null)
                {
                    var res = await _unitOfWork.ApplicationUser.DeletePhoto(user.PublicId);
                    if (res == "ok")
                    {
                        user.ProfilePic = null;
                        user.PublicId = null;
                    }
                    return Ok();
                }
                else
                    return BadRequest("User do not have photo");
            }
            var admin = await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId);
            if (admin != null)
            {
                if (admin.ProfilePic != null)
                {
                    var res = await _unitOfWork.Admin.DeletePhoto(admin.PublicId);
                    if (res == "ok")
                    {
                        admin.ProfilePic = null;
                        admin.PublicId = null;
                    }
                    return Ok();
                }
                else
                    return BadRequest("User do not have photo");
            }
            return Unauthorized();
        }

    }
}
