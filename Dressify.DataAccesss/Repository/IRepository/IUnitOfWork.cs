using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {   //Repository
        public IApplicationUserRepository ApplicationUser { get; }
        public IProductRepository Product { get; }
        public IWishListRepository WishList { get; }
        public IProductRateRepository ProductRate { get; }
        public IProductQuestionRepository ProductQuestion { get; }
        public IProductReportRepository ProductReport { get; }
        public IPenaltyRepository Penalty { get; }

        public IProductActionRepository ProductAction { get; }
        public IProductImageRepository ProductImage{ get; }
        public ISuperAdminRepository   SuperAdmin { get; }
        public IAdminRepository Admin { get; }

        public IShoppingCartRepository  ShoppingCart { get; }
        public IOrderRepository Order { get; }
        public IOrderDetailsRepository OrderDetails { get; }
        public IPayBillRepository PayBill { get; }


        //Functions
        int Save();
        string getUID();
        Task<AuthDto> CreateJwtToken(AdminTokenRequestDto model);
        void Unsuspend();
        void SendEmail(Message message);
        decimal CalculatePrice(int quantity, float price, float? sale = 0);

    }
}
