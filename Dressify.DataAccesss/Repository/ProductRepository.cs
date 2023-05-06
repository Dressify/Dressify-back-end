using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync(int? reportCountThreshold = 10)
        {
            reportCountThreshold ??= 10; // if reportCountThreshold is null, set its value to 10

            var products = await _context.Products.Include(p => p.Reports)
                .Where(p => p.Reports.Count >= reportCountThreshold
                && p.Reports.Any(pr => pr.ReportStatus == true)
                && p.IsSuspended == false)
                .ToListAsync();

            return products;
        }
    }
}
