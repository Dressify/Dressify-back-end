using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using MailKit.Search;
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
                productsQuery = productsQuery.Where(p => p.Price >= (p.Sale!=null? model.MinPrice.Value / (1 - p.Sale / 100): model.MinPrice.Value) );
            }

            if (model.MaxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= (p.Sale != null ? model.MaxPrice.Value / (1 - p.Sale / 100) : model.MaxPrice.Value));
            }

            if (!string.IsNullOrEmpty(model.Gender))
            {
                productsQuery = productsQuery.Where(p => p.Type.Trim().ToLower() == model.Gender.Trim().ToLower());
            }

            if (!string.IsNullOrEmpty(model.Category))
            {
                productsQuery = productsQuery.Where(p => p.Category.Trim().ToLower() == model.Category.Trim().ToLower());
            }

            if (!string.IsNullOrEmpty(model.SubCategory))
            {
                productsQuery = productsQuery.Where(p => p.SubCategory.Trim().ToLower() == model.SubCategory.Trim().ToLower());
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Trim().ToLower().Contains(model.SearchTerm.Trim().ToLower()) || (p.Description != null && p.Description.Trim().ToLower().Contains(model.SearchTerm.Trim().ToLower())));
            }

            var totalCount =  productsQuery.Count();
            if (!productsQuery.Any())
            {
                NoContent();
            }
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
    
            var categories = distinctValues.Where(p => p.Category != null).Select(p => p.Category).Distinct().ToList();
            var subCategories = distinctValues.Where(p => p.SubCategory != null).Select(p => p.SubCategory).Distinct().ToList();
            var types = distinctValues.Where(p => p.Type != null).Select(p => p.Type).Distinct().ToList();

            return Ok(new { categories, subCategories, types });
        }

       
        [HttpGet("GetSuspendedProducts")]
        [Authorize]
        public async Task<IActionResult> GetSuspendedProducts([FromQuery] int? PageNumber, [FromQuery] int? PageSize,[FromQuery] string? SearchTerm)
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
            PageNumber ??= 1;
            PageSize ??= 10;
            var skip = (PageNumber - 1) * PageSize;
            var productsQuery = await _unitOfWork.Product.FindAllAsync(u => u.IsSuspended == true, new[] { "ProductImages", "ProductRates" });
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Trim().ToLower().Contains(SearchTerm.Trim().ToLower()) || (p.Description != null && p.Description.Trim().ToLower().Contains(SearchTerm.Trim().ToLower())));
            }

            var totalCount = productsQuery.Count();
            if (!productsQuery.Any())
            {
                NoContent();
            }
            var products = productsQuery
                .Skip(skip.Value)
                .Take(PageSize.Value)
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
            return Ok(new { Count = totalCount, ProductsWithAvgRates = productsWithAvgRates });
        }

        [HttpGet("GetNewArrivals")]
        public async Task<IActionResult> GetNewArrivals()
        {
            var products = await _unitOfWork.Product.newArrivals();
            if (products == null)
                return NotFound();
            var productsWithAvgRates = products.Select(p => new
            {
                Product = p,
                AvgRate = _unitOfWork.ProductRate.CalculateAverageRate(p.ProductRates)
            }).ToList();
            return Ok(new { ProductsWithAvgRates = productsWithAvgRates });
        }

    }
}

