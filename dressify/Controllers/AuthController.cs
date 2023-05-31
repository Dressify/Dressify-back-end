﻿using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Newtonsoft.Json.Linq;
using NuGet.Common;
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
                return BadRequest(result.Messages);
            return Ok(result);
        }
        [HttpPost("VendorRegister")]
        public async Task<IActionResult> VendorRegisterAsync([FromBody] VendorRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _unitOfWork.ApplicationUser.VendorRegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Messages);
            return Ok(result);
        }


        // login for customer and vendor
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

        [Authorize]
        //Modify Photo For Vendor ,and Customer 
        [HttpPut("ModifyPhoto")]
        public async Task<IActionResult> ModifyPhoto(IFormFile photo)
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
                return Ok(result);
            }
        return Unauthorized();
        }
        [Authorize]
        //Delete Photo For Vendor ,and Customer 
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
                        _unitOfWork.Save();
                    }
                    return Ok();
                }
                else
                    return BadRequest("User do not have photo");
            }
            return Unauthorized();
        }

        //replace real email with your target mail
        //[HttpGet("TestEmail")]
        //public async Task<IActionResult> Test()
        //{
        //    var message = new Message(new string[] { "RealEmail@gmail.com"},"Test","<h1>nice</h1>");
        //    _unitOfWork.SendEmail(message);
        //    return Ok("done");
        //}

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user =await _unitOfWork.ApplicationUser.FindAsync(u => u.Email == email);
            if (user != null)
            {
               var token= await _unitOfWork.ApplicationUser.ResetPasswordTokenAsync(user);
               //var link = Url.Action(nameof(ResetPassword), "Auth", new { token, email = user.Email }, Request.Scheme);
               var message = new Message(new string[] { user.Email }, "Forgot Password Link", EmailBody.ResetPasswordEmail(token,email),true);
               _unitOfWork.SendEmail(message);
               return Ok();
            }
            return BadRequest("Error happened while sending link or Email doesn`t exist");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _unitOfWork.ApplicationUser.FindAsync(u => u.Email == model.Email);
            if (user != null)
            {
                var ResetResult = await _unitOfWork.ApplicationUser.ResetPasswordAsync(user, model.Token, model.Password);
                if(!ResetResult.Succeeded)
                {
                    foreach(var error in ResetResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok("Password has been changed");
            }
            return BadRequest("Error happened while sending link or Email doesn`t exist");
        }


    }
}
