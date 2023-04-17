using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System.ComponentModel;
using System.Drawing;
using System.Security.Claims;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorAndSalesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public VendorAndSalesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //[Authorize(Roles =SD.Role_Sales  +  SD.Role_Vendor)]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm]CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("product Data Not Valid");
            var product = new Product
            {
                VendorId = dto.VendorId,
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                Sale = dto.Sale,
                Rentable=dto.Rentable,
                Color=dto.Color,
                Category = dto.Category,
                SubCategory = dto.SubCategory,
                Type = dto.Type,
            };
            _unitOfWork.Product.AddAsync(product);
            _unitOfWork.Save();
           foreach(var img in dto.Photos)
            { 
               CreatePhotoDto result =await _unitOfWork.ProductImage.AddPhoto(img);
                var extension = System.IO.Path.GetExtension(img.FileName);

                var productImg = new ProductImage
                {
                    ProductId = product.ProductId,
                    PublicId = result.PublicId,
                    ImageUrl=result.Url,
                    ImageExtension= extension,
                };
                _unitOfWork.ProductImage.Add(productImg);
                _unitOfWork.Save();
            }
            return Ok();
        }
    }
}