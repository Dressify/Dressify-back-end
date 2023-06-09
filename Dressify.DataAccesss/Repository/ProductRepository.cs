﻿using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Product = Dressify.Models.Product;

namespace Dressify.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetProducts(int? reportCountThreshold)
        {
            reportCountThreshold ??= 10; // if reportCountThreshold is null, set its value to 10

            var products = _context.Products.Include(p => p.Reports).Include(p => p.ProductImages)
                .Where(p => p.Reports.Count >= reportCountThreshold
                && p.Reports.Any(pr => pr.ReportStatus == true)
                && p.IsSuspended == false);
            return products;
        }
        //public IEnumerable<Product> FindAll(Expression<Func<Product, bool>> criteria, int? skip, int? take, double? minPrice, double? maxPrice, string? gender, string? category, string[] includes = null)
        //{
        //    IQueryable<Product> query = _context.Set<Product>().Where(criteria);
        //    if (minPrice.HasValue)
        //    {
        //        query = query.Where(p => p.Price >= minPrice.Value);
        //    }

        //    if (maxPrice.HasValue)
        //    {
        //        query = query.Where(p => p.Price <= maxPrice.Value);
        //    }

        //    if (!string.IsNullOrEmpty(gender))
        //    {
        //        query = query.Where(p => p.Type == gender);
        //    }

        //    if (!string.IsNullOrEmpty(category))
        //    {
        //        query = query.Where(p => p.Category == category);
        //    }

        //    if (skip.HasValue)
        //        query = query.Skip(skip.Value);

        //    if (take.HasValue)
        //        query = query.Take(take.Value);
        //    if (includes != null)
        //        foreach (var include in includes)
        //            query = query.Include(include);

        //    return query.ToList();
        //}
        //public async Task<IEnumerable<Product>> FindAllProductAsync(Expression<Func<Product, bool>> criteria, int? skip, int? take, double? minPrice, double? maxPrice, string? gender, string? category, string[] includes = null)
        //{
        //    IQueryable<Product> query = _context.Set<Product>().Where(criteria);
        //    if (minPrice.HasValue)
        //    {
        //        query = query.Where(p => p.Price >= minPrice.Value);
        //    }

        //    if (maxPrice.HasValue)
        //    {
        //        query = query.Where(p => p.Price <= maxPrice.Value);
        //    }

        //    if (!string.IsNullOrEmpty(gender))
        //    {
        //        query = query.Where(p => p.Type == gender);
        //    }

        //    if (!string.IsNullOrEmpty(category))
        //    {
        //        query = query.Where(p => p.Category == category);
        //    }
           
        //    if (skip.HasValue)
        //        query = query.Skip(skip.Value);
        //    if (take.HasValue)
        //        query = query.Take(take.Value);
        //    if (includes != null)
        //        foreach (var include in includes)
        //            query = query.Include(include);

        //    return await query.ToListAsync();
        //}

        public async Task<List<Product>> newArrivals()
        {
            var products = _context.Products.OrderByDescending(p => p.ProductId)
            .Take(8)
            .Include(p =>p.ProductImages)
            .Include(p => p.ProductRates)
            .ToList();
            return products;
        }

        public async Task<Product> LastProduct()
        {
            var lastRecord = await _context.Products
            .OrderByDescending(p => p.ProductId)
            .FirstOrDefaultAsync();
            return lastRecord;
        }

        public async Task<List<Product>> GetProductsOnOrder(List<int> productsIds)
        {
            var products = await _context.Products.Where(p => productsIds.Contains(p.ProductId))
                .Include(p => p.ProductImages)
                .Include(p => p.ProductRates).ToListAsync();
            products = products.OrderBy(p => productsIds.IndexOf(p.ProductId)).ToList();
            return products;
        }

        public async Task<Dictionary<string, double>> ProductsRated(string customerId)
        {
            var categories = new List<string>{
             "pants", "jeans", "shirt", "t_shirt", "jacket", "coat", "hoodies", "sweatshirts",
             "blazer", "sneaker", "boot", "oxford", "blouseClean", "skirtClean", "tie"};

            var averageRatingsByCategory = new Dictionary<string, double>();

            foreach (var category in categories)
            {
                var averageRating = await _context.Products
                    .Where(p => (p.Category == category || p.SubCategory == category) && p.ProductRates.Any(pr => pr.CustomerId == customerId))
                    .SelectMany(p => p.ProductRates)
                    .Where(pr => pr.CustomerId == customerId)
                    .AverageAsync(pr => pr.Rate);

                averageRatingsByCategory[category] = averageRating??0.0;
            }

            // Replace null values with 0
            foreach (var category in categories)
            {
                if (!averageRatingsByCategory.ContainsKey(category))
                {
                    averageRatingsByCategory[category] = 0;
                }
            }

            return averageRatingsByCategory;
        }

    }
}
    