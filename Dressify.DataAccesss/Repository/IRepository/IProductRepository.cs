using Dressify.Models;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetProductsAsync(int? reportCountThreshold);
    }
}
