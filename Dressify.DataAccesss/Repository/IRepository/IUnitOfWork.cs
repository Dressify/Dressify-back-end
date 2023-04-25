﻿using System;
using System.Collections.Generic;
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
        public IProductImageRepository ProductImage{ get; }
        public ISuperAdminRepository   SuperAdmin { get; }
        public IShoppingCartRepository  ShoppingCart { get; }

        //Functions
        int Save();
        string getUID();
    }
}
