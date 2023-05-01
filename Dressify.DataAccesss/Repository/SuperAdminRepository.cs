using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class SuperAdminRepository : Repository<SuperAdmin>, ISuperAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public SuperAdminRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SuperAdmin> AddSuperAdminAsync(SuperAdmin sAdmin, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            sAdmin.PasswordHash = passwordHash;
            sAdmin.PasswordSalt = passwordSalt;
            var suberAdmin = new SuperAdmin
            {
                UserName= sAdmin.UserName,
                PasswordHash=passwordHash,
                PasswordSalt=passwordSalt,
            };
            _context.SuperAdmins.Add(suberAdmin);
            _context.SaveChanges();
            return suberAdmin;
        }
        
        public async Task<Admin> CreateAdminAsync(AddAdminDto adminDto)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(adminDto.Password, out passwordHash, out passwordSalt);
            var admin = new Admin
            {
                AdminName = adminDto.AdminName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email=adminDto.Email,
            };
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        private void CreatePasswordHash(string password,out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
