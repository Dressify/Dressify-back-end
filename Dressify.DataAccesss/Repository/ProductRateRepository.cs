using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class ProductRateRepository : Repository<ProductRate>, IProductRateRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRateRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public double CalculateAverageRate(IEnumerable<ProductRate> rates)
        {
            if(!rates.Any())
                return 0;
            var rateSum = rates.Where(r => r.Rate.HasValue).Sum(r => r.Rate.Value);
            var count = rates.Count(r => r.Rate.HasValue);
            return count == 0 ? 0 : (double)rateSum / count;
        }
    }
}
