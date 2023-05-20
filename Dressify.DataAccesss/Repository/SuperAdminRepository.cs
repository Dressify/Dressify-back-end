using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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



        //public async Task<ClaimsIdentity> getID(string token)
        //{
        //    if(token == null)
        //    {
        //        return null;
        //    }
        //    var handler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(_jwt.Key); // replace with your own secret key
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(key),
        //        ValidIssuer = _jwt.Issuer,
        //        ValidAudience = _jwt.Audience,
        //    };
        //    try
        //    {
        //        var claimsPrincipal = handler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        //        var identity = claimsPrincipal.Identity as ClaimsIdentity;
        //        return identity;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any exceptions that occur during token validation
        //        // For example, if the token is expired or invalid
        //        return null;
        //    }
        //}


    }
}
