﻿using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dressify.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager, IOptions<CloudinarySettings> cloudinary, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _cloudinaryConfig = cloudinary;
            ApplicationUser = new ApplicationUserRepository(context, userManager, jwt, roleManager);
            Product = new ProductRepository(_context);
            WishList = new WishListRepository(_context);
            ProductQuestion = new ProductQuestionRepository(_context);
            ProductRate = new ProductRateRepository(_context);
            SuperAdmin = new SuperAdminRepository(_context);
            ProductImage = new ProductImageRepository(_context, cloudinary);
            ShoppingCart = new ShoppingCartRepository(_context);
            _httpContextAccessor = httpContextAccessor;
        }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IProductRepository Product { get; private set; }
        public IWishListRepository WishList { get; private set; }
        public IProductQuestionRepository ProductQuestion { get; private set; }
        public IProductRateRepository ProductRate { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public ISuperAdminRepository   SuperAdmin { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }



        public int Save()
        {
            return _context.SaveChanges();
        }
        public string getUID()
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var uId = claimsIdentity.FindFirst("uid").Value;
            return uId;
        }
    }
}
