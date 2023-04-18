using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager, IOptions<CloudinarySettings> cloudinary)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _cloudinaryConfig =cloudinary;
            ApplicationUser = new ApplicationUserRepository(context, userManager, jwt, roleManager);
            Product = new ProductRepository(_context);
            WishList = new WishListRepository(_context);
            ProductQuestion = new ProductQuestionRepository(_context);
            ProductRate = new ProductRateRepository(_context);
            SuperAdmin = new SuperAdminRepository(_context);
            ProductImage = new ProductImageRepository(_context, cloudinary);
        }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IProductRepository Product { get; private set; }
        public IWishListRepository WishList { get; private set; }
        public IProductQuestionRepository ProductQuestion { get; private set; }
        public IProductRateRepository ProductRate { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public ISuperAdminRepository   SuperAdmin { get; private set; }



        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
