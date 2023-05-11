using Dressify.DataAccess.Helpers;
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
using Dressify.DataAccess.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Dressify.Utility;
using MimeKit;
using MailKit.Net.Smtp;

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
        private readonly EmailConfiguration _emailConfiguration;



        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager,IOptions<EmailConfiguration> emailConfiguration, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager, IOptions<CloudinarySettings> cloudinary, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _cloudinaryConfig = cloudinary;
            _emailConfiguration = emailConfiguration.Value;

            ApplicationUser = new ApplicationUserRepository(context, userManager, jwt, roleManager, cloudinary);
            Product = new ProductRepository(_context);
            WishList = new WishListRepository(_context);
            ProductQuestion = new ProductQuestionRepository(_context);
            ProductRate = new ProductRateRepository(_context);
            ProductReport = new ProductReportRepository(_context);
            Penalty = new PenaltyRepository(_context);
            ProductAction = new ProductActionRepository(_context);
            SuperAdmin = new SuperAdminRepository(_context);
            Admin = new AdminRepository(_context, cloudinary);

            ProductImage = new ProductImageRepository(_context, cloudinary);
            ShoppingCart = new ShoppingCartRepository(_context);
            Order = new OrderRepository(_context);
            OrderDetails = new OrderDetailsRepository(_context);
            PayBill = new PayBillRepository(_context);

            _httpContextAccessor = httpContextAccessor;
        }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IProductRepository Product { get; private set; }
        public IWishListRepository WishList { get; private set; }
        public IProductQuestionRepository ProductQuestion { get; private set; }
        public IProductRateRepository ProductRate { get; private set; }
        public IProductReportRepository ProductReport { get; private set; }
        public IPenaltyRepository Penalty { get; private set; }

        public IProductActionRepository ProductAction { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public ISuperAdminRepository   SuperAdmin { get; private set; }
        public IAdminRepository Admin { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IPayBillRepository PayBill { get; private set; }




        public int Save()
        {
            return _context.SaveChanges();
        }
        public string getUID()
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            if (claimsIdentity == null)
                return null;
            var uId = claimsIdentity.FindFirst("uid").Value;
            return uId;
        }


        public async Task<AuthDto> CreateJwtToken(AdminTokenRequestDto model)
        {
            var authModel = new AuthDto();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", model.ID),
                new Claim("Role", SD.Role_Admin),

            };
            var identity = new ClaimsIdentity(claims, "MyApplication");
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);


            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Username = model.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            return authModel;

        }

        public void Unsuspend()
        {
            var now = DateTime.UtcNow;
            var suspendedProducts =  _context.Products.Where(p => p.IsSuspended && p.SuspendedUntil.HasValue && p.SuspendedUntil.Value <= now).ToList();
            var suspendedVendors = _context.Users.Where(p => p.IsSuspended && p.SuspendedUntil.HasValue && p.SuspendedUntil.Value <= now).ToList();

            foreach (var product in suspendedProducts)
            {
                product.IsSuspended = false;
                product.SuspendedUntil = null;

                _context.Products.Update(product);
            }

            foreach (var vendor in suspendedVendors)
            {
                vendor.IsSuspended = false;
                vendor.SuspendedUntil = null;

                _context.Users.Update(vendor);
            }
             _context.SaveChanges();
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);

                client.Send(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
