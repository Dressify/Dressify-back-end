using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {   
        private UserManager<ApplicationUser> userManager;
        private IOptions<JWT> jwt;
        private RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager, IOptions<CloudinarySettings> cloudinary) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _cloudinaryConfig = cloudinary;
        }
        public async Task<AuthDto> CustomerRegisterAsync(CustRegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                return new AuthDto { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(dto.Username) is not null)
                return new AuthDto { Message = "Username is already registered!" };
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthDto { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, SD.Role_Customer);

            var jwtSecurityToken = await CreateJwtToken(user);
            await _userManager.UpdateAsync(user);
            return new AuthDto
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Role = SD.Role_Customer,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                ImgUrl = user.ProfilePic,
            };
        }
        public async Task<AuthDto> RegisterAsync(RegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                return new AuthDto { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(dto.Username) is not null)
                return new AuthDto { Message = "Username is already registered!" };
            if(! await _roleManager.RoleExistsAsync(dto.Role))
                return new AuthDto { Message = "This Role not exist" };
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FName = dto.FName,
                LName = dto.LName,
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthDto { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            var jwtSecurityToken = await CreateJwtToken(user);
            await _userManager.UpdateAsync(user);
            return new AuthDto
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Role = dto.Role,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                ImgUrl=user.ProfilePic,
            };

        }
        public async Task<AuthDto> GetTokenAsync(TokenRequestDto model)
        {
            var authModel = new AuthDto();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Role = rolesList.ToList().FirstOrDefault();
            authModel.ImgUrl = user.ProfilePic;
            return authModel;
        }

        public async Task<ApplicationUser> GetUserAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<string> GetRoleAsync(ApplicationUser user)
        {
            var roleList= await _userManager.GetRolesAsync(user);

            return roleList.FirstOrDefault();
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<CreatePhotoDto> AddPhoto(IFormFile file)
        {
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.Key,
            _cloudinaryConfig.Value.Secret
                );
            _cloudinary = new Cloudinary(acc);
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            var CreatedPhoto = new CreatePhotoDto
            {
                Url = uploadResult.Url.ToString(),
                PublicId = uploadResult.PublicId
            };
            return CreatedPhoto;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.Key,
            _cloudinaryConfig.Value.Secret
                );
            _cloudinary = new Cloudinary(acc);
            var deleteParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deleteParams);
            return result.Result;
        }

    }
}
    