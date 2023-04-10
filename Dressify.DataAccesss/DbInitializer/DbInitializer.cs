﻿using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dressify.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public void Initialize()
        {

            //migrations if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex) 
            { 
            }

            //create roles if they not exist
            if (!_roleManager.RoleExistsAsync(SD.Role_Vendor).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Vendor)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Sales)).GetAwaiter().GetResult();
                 
                //add Dressify Vendor to DB
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName="Dressify",
                    Email="Dressify@gmail.com",
                    PhoneNumber="011",
                    storeName="DressifyStore",
                },"Admin123*").GetAwaiter().GetResult();
                ApplicationUser user = _context.Users.FirstOrDefault(u => u.Email == "Dressify@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Sales).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
