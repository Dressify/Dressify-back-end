using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public int? DecrementCount(ShoppingCart cart, int? count)
        {
            cart.Quantity -= count;
            return cart.Quantity;
        }

        public int? IncrementCount(ShoppingCart cart, int? count)
        {
            cart.Quantity += count;
            return cart.Quantity;
        }


    }
}
