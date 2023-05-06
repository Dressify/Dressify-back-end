﻿using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public VendorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllQuestions")]
        [Authorize(Roles = SD.Role_Vendor)]
        public async Task<ActionResult<IEnumerable<ProductQuestion>>> GetAllQuestions()
        {
            var uId = _unitOfWork.getUID();
            var vendor = await _unitOfWork.ApplicationUser.GetUserAsync(uId);
            if (vendor == null)
            {
                return NotFound("vendor does not exist");
            }
            var questions = await _unitOfWork.ProductQuestion.FindAllAsync(u => u.VendorId == uId, new[] {"Product"});
            if (questions.Any())
            {
                return NoContent();
            }
            return Ok(questions);
        }

        [HttpPut("AnswearQuestion")]
        public async Task<IActionResult> AnswearQuestion(AnswearDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(_unitOfWork.getUID());
            var product = await _unitOfWork.Product.GetByIdAsync(obj.ProductId);
            var question = await _unitOfWork.ProductQuestion.GetByIdAsync(obj.QuestionId);

            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            if (question == null)
            {
                return BadRequest("question does not exist");
            }
            if (obj.Answear == null || obj.Answear == "")
            {
                return BadRequest("What's your Answear?!");
            }

            question.Answear=obj.Answear;
            question.VendorId=user.Id;

            _unitOfWork.ProductQuestion.Update(question);
            _unitOfWork.Save();
            return Ok(question);
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("product Data Not Valid");
            var product = new Product
            {
                VendorId = _unitOfWork.getUID(),
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                Sale = dto.Sale,
                Rentable = dto.Rentable,
                Color = dto.Color,
                Category = dto.Category,
                SubCategory = dto.SubCategory,
                Type = dto.Type,
            };
            await _unitOfWork.Product.AddAsync(product);
            _unitOfWork.Save();
            foreach (var img in dto.Photos)
            {
                CreatePhotoDto result = await _unitOfWork.ProductImage.AddPhoto(img);
                var extension = System.IO.Path.GetExtension(img.FileName);

                var productImg = new ProductImage
                {
                    ProductId = product.ProductId,
                    PublicId = result.PublicId,
                    ImageUrl = result.Url,
                    ImageExtension = extension,
                };
                _unitOfWork.ProductImage.Add(productImg);
                _unitOfWork.Save();
            }
            return Ok();
        }


        [HttpGet("GetSuspendedVendor")]
        [Authorize]
        public async Task<IActionResult> GetSuspendedVendor()
        {
            var uId = _unitOfWork.getUID();
            if (await _unitOfWork.Admin.FindAsync(u => u.AdminId == uId) == null)
            {
                return Unauthorized();
            }
            var vendors = await _unitOfWork.ApplicationUser.FindAllAsync(u => u.IsSuspended == true);
            return Ok(vendors);
        }

    }
}