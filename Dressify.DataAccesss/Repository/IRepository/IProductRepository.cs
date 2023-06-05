using Dressify.DataAccess.Dtos;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProducts(int? reportCountThreshold);
        Task<List<Product>> newArrivals();
        Task<Dictionary<string, double>> ProductsRated(string customerId);

        //IEnumerable<Product> FindAll(Expression<Func<Product, bool>> criteria, int? take, int? skip,
        // double? minPrice, double? maxPrice, string? gender, string? category,
        // string[] includes = null);
        //Task<IEnumerable<Product>> FindAllProductAsync(Expression<Func<Product, bool>> criteria, int? skip, int? take,
        // double? minPrice, double? maxPrice, string? gender, string? category,
        // string[] includes = null);
    }
}
