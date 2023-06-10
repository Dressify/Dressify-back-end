using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public int OrdersQuantity(IEnumerable<OrderDetails> Details)
        {
            int sum = 0;
            foreach (var item in Details)
            {
                sum += item.Quantity.Value;
            }
            return sum;
        }
        public  void returnProductQuantity(int orderid)
        {
            var OrderDetails = _context.OrdersDetails.Where(o => o.OrderId == orderid).ToList();
            foreach(var item in OrderDetails)
            {
               var  product= _context.Products.Find(item.ProductId);
                product.Quantity +=item.Quantity.Value;
                _context.Update(product);
            }

        }
    }
}