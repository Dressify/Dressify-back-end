using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Linq;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //[HttpGet("GetProductspage")]
        //public async Task<IActionResult> GetProductsPage([FromQuery] GetProductsDto model)
        //{
        //    if (model.PageNumber <= 0 || model.PageNumber <= 0)
        //    {
        //        return BadRequest("Page number and page size must be positive integers.");
        //    }
        //    var skip = (model.PageNumber - 1) * model.PageSize;
        //    var products = await _unitOfWork.Product.FindAllProductAsync(u => u.IsSuspended == false,
        //        skip, model.PageSize, model.MinPrice, model.MaxPrice, model.Gender, model.Category,
        //        new[] { "Vendor", "ProductImages", "ProductRates" });
        //    var count = await _unitOfWork.Product.CountAsync();

        //    var productsWithAvgRates = products.Select(p => new
        //    {
        //        Product = p,
        //        Count = count,
        //        AvgRate = _unitOfWork.ProductRate.CalculateAverageRate(p.ProductRates)
        //    }).ToList();
        //    return Ok(productsWithAvgRates);
        //}

        [HttpGet("GetProductspage")]
        public async Task<IActionResult> GetProductsPage([FromQuery] GetProductsDto model)
        {
            if (model.PageNumber <= 0 || model.PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            model.PageNumber ??= 1;
            model.PageSize ??= 10;
            var skip = (model.PageNumber - 1) * model.PageSize;

            var productsQuery =await _unitOfWork.Product.FindAllAsync(u=>u.IsSuspended==false,new[] { "ProductImages", "ProductRates" });

            if (model.MinPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= model.MinPrice.Value);
            }

            if (model.MaxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= model.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(model.Gender))
            {
                productsQuery = productsQuery.Where(p => p.Type == model.Gender);
            }

            if (!string.IsNullOrEmpty(model.Category))
            {
                productsQuery = productsQuery.Where(p => p.Category == model.Category);
            }

            if (!string.IsNullOrEmpty(model.SubCategory))
            {
                productsQuery = productsQuery.Where(p => p.SubCategory == model.SubCategory);
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(model.SearchTerm) || p.Description.Contains(model.SearchTerm));
            }

            var totalCount =  productsQuery.Count();
            var products =  productsQuery
                .Skip(skip.Value)
                .Take(model.PageSize.Value)
                .ToList();
            if (!products.Any())
            {
                NoContent();
            }
            var productsWithAvgRates = products.Select(p => new
            {
                Product = p,
                AvgRate = _unitOfWork.ProductRate.CalculateAverageRate(p.ProductRates)
            }).ToList();
            

            return Ok(new{ Count = totalCount, ProductsWithAvgRates= productsWithAvgRates });
        }


        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProduct([FromQuery] int id)
        {

            var product = await _unitOfWork.Product.FindAsync(p => p.ProductId == id,
                new[] { "Vendor", "ProductImages", "Questions" , "ProductRates" });
            if (product == null)
                return NotFound();
            if (product.IsSuspended)
            {
                return BadRequest("product is suspended untill: " + product.SuspendedUntil.ToString());
            }
            var Details = new ProductDetailsDto
            {
                Product = product,
                AverageRate = _unitOfWork.ProductRate.CalculateAverageRate(product.ProductRates),
                Quantity = 1,
            };
            return Ok(Details);
        }

        [HttpGet("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews([FromQuery] int id)
        {
            var product = await _unitOfWork.Product.FindAsync(p => p.ProductId == id); 
            if (product == null)
                return NotFound();
            var ratingList = await _unitOfWork.ProductRate.FindAllAsync(r => r.ProductId == id , new[] { "ApplicationUser" } );
            var count = ratingList.Count();
            decimal? sum=0;
            var productRates = new ProductRatesDto() 
            {
                Count = count,
            };    

            var rates = new List<CustomerRateDto>();
            if (count == 0)
            {
                productRates.customerRates = rates;
                productRates.average = 0;
                return Ok(productRates);
            }
            foreach (var r in ratingList)
            {
                var result = new CustomerRateDto()
                {
                    rate =  r.Rate,
                    RateComment = r.RateComment,
                    CustomerName =r.ApplicationUser.UserName,
                };
                sum += r.Rate;
                rates.Add(result);
            }
            var ave = sum / productRates.Count;
            productRates.average = ave;
            productRates.customerRates = rates;
            return Ok(productRates);
        }


        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var distinctValues = _unitOfWork.Product.GetAll()
              .Select(p => new { p.Category, p.SubCategory, p.Type }).ToList();
            if (!distinctValues.Any())
            {
                return NoContent();
            }
    
            var categories = distinctValues.Select(p => p.Category).Distinct().ToList();
            var subCategories = distinctValues.Select(p => p.SubCategory).Distinct().ToList();
            var types = distinctValues.Select(p => p.Type).Distinct().ToList();

            return Ok(new { categories, subCategories, types });
        }

        //[HttpGet("SearchProducts")]
        //public async Task<IActionResult> SearchProducts([FromQuery] string searchTerm)
        //{
        //    var products = await _unitOfWork.Product.FindAllAsync(p =>
        //        (p.ProductName.Contains(searchTerm) || (p.Description != null && p.Description.Contains(searchTerm)))
        //        && p.IsSuspended == false, new[] { "ProductImages" });

        //    if (!products.Any())
        //    {
        //        return NotFound();
        //    }
        //    return Ok(products);
        //}

        [HttpGet("GetSuspendedProducts")]
        [Authorize]
        public async Task<IActionResult> GetSuspendedProducts([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            if (PageNumber <= 0 || PageSize <= 0)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }
            var skip = (PageNumber - 1) * PageSize;
            var products = await _unitOfWork.Product.FindAllAsync(u => u.IsSuspended == true, skip, PageSize);
            var count = await _unitOfWork.Product.CountAsync(u => u.IsSuspended == true);

            return Ok(new { Count = count, Products = products });
        }


    }
}

