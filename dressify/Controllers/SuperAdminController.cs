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
        //        // During login, create a ClaimsIdentity object for the user and add claims to it
        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, "John Doe"),
        //    new Claim(ClaimTypes.Email, "john.doe@example.com"),
        //    new Claim("SubscriptionLevel", "Premium")
        //};

        //        var identity = new ClaimsIdentity(claims, "MyApplication");

        //        // Use the ClaimsIdentity object to authenticate the user and generate an authentication token
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes("mysecretkey"); // replace with your own secret key
        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = identity,
        //            Expires = DateTime.UtcNow.AddDays(7),
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //        };

        //        var token = tokenHandler.CreateToken(tokenDescriptor);
        //        var authenticationToken = tokenHandler.WriteToken(token);

        //        // Include the ClaimsIdentity object in the authentication token
        //        var payload = new Dictionary<string, object>
        //{
        //    { "identity", identity }
        //};

        //        authenticationToken += "." + Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));

        //// Pass the authentication token to another API or service that requires authentication
        //// and access the claims in the ClaimsIdentity object
        //var handler = new JwtSecurityTokenHandler();
        //        var tokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false
        //        };

        //        var claimsPrincipal = handler.ValidateToken(authenticationToken, tokenValidationParameters, out SecurityToken validatedToken);

        //        // Access the claims from the ClaimsIdentity object
        //        var subscriptionLevel = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "SubscriptionLevel")?.Value;

        //        // During logout, include a flag or message in the response to indicate that the user has logged out
        //        // and dispose of the ClaimsIdentity object in the second API
        //        var response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StringContent("Logout successful.");

        //        // Include the ClaimsIdentity object in the response so that it can be disposed of in the second API
        //        payload = new Dictionary<string, object>
        //{
        //    { "disposeIdentity", true }
        //};

        //    response.Headers.Add("X-MyApplication-Payload", Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload))));

        //return response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SuperAdminController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
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