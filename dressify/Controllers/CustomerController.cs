using Dressify.DataAccess.Repository.IRepository;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dressify.DataAccess.Dtos;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Dressify.Models;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("addToWishList")]
        public async Task<IActionResult> AddToWishList(WishListDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(obj.CustomerId);
            var product = await _unitOfWork.Product.GetByIdAsync(obj.ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            var item = new WishList
            {
                CustomerId = obj.CustomerId,
                ProductId = obj.ProductId,
            };
            await _unitOfWork.WishList.AddAsync(item);
            _unitOfWork.Save();
            return Ok(obj);
        }

        [HttpPost("CustomerRate")]
        public async Task<IActionResult> CustomerRateProduct(RateDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(obj.CustomerId);
            var product = _unitOfWork.Product.GetById(obj.ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            if(obj.rate <1 && obj.rate > 5) 
            {
                return BadRequest("Rating should be between 1 to 5 ");
            }
            var result = _unitOfWork.ProductRate.Find(r => r.ProductId==obj.ProductId && r.CustomerId==obj.CustomerId);

            if (result is not null )
            {
                return BadRequest("This product has already been rated by the customer ");
            }
            else
            {
                var item = new ProductRate
                {
                    CustomerId = obj.CustomerId,
                    ProductId = obj.ProductId,
                    rate =obj.rate,
                    RateComment = obj.RateComment,
                    //include isPurchased after adding Order 
                    Date=DateTime.Now,
                };
                await _unitOfWork.ProductRate.AddAsync(item);
                _unitOfWork.Save();

            } 
            return Ok(obj);
        }


        [HttpPost("AskQuestion")]
        public async Task<IActionResult> AskQuestion(QuestionDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(obj.CustomerId);
            var product = await _unitOfWork.Product.GetByIdAsync(obj.ProductId);
            if (user == null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            if (obj.Question == null||obj.Question=="")
            {
                return BadRequest("What's your Question?!");
            }
            var item = new ProductQuestion
            {
                Question = obj.Question,
                CustomerId = obj.CustomerId,
                ProductId = obj.ProductId,
            };
            await _unitOfWork.ProductQuestion.AddAsync(item);
            _unitOfWork.Save();
            return Ok(obj);
        }
    }
}
