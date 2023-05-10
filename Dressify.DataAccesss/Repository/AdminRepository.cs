using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dressify.DataAccess.Helpers;
using Microsoft.Extensions.Options;

namespace Dressify.DataAccess.Repository
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public AdminRepository(ApplicationDbContext context, IOptions<CloudinarySettings> cloudinary) : base(context)
        {
            _context = context;
            _cloudinaryConfig = cloudinary;
        }


        public async Task<AuthDto> CreateAdminAsync(AddAdminDto adminDto)
        {
            if (await _context.Admins.FirstOrDefaultAsync(u => u.Email == adminDto.Email) is not null)
                return new AuthDto { Message = "Email is already registered!" };

            if (await _context.Admins.FirstOrDefaultAsync(u => u.AdminName == adminDto.AdminName) is not null)
                return new AuthDto { Message = "Username is already registered!" };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(adminDto.Password, out passwordHash, out passwordSalt);
            var admin = new Admin
            {
                AdminName = adminDto.AdminName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = adminDto.Email,
                ProfilePic=adminDto.ProfilePic,
                PublicId = adminDto.PublicId,

            };
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();

            return new AuthDto
            {
                Email = admin.Email,
                IsAuthenticated = false,
                Username = admin.AdminName,
                ImgUrl = admin.ProfilePic,
                Message = ""
            };
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

