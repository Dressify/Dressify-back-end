using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
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

        
    }
}

