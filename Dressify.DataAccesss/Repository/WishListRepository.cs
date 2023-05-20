using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class WishListRepository : Repository<WishList>,IWishListRepository
    {
        private readonly ApplicationDbContext _context;
        public WishListRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
