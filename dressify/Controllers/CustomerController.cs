using Dressify.DataAccess.Repository.IRepository;
using Dressify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dressify.DataAccess.Dtos;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

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
        [Authorize(Roles =SD.Role_Customer)]
        public async Task<IActionResult> AddToWishList(WishListDto obj)
        {
            var user = await _unitOfWork.ApplicationUser.GetUserAsync(obj.CustomerId);
            var product = _unitOfWork.Product.GetById(obj.ProductId);
            if (user==null)
            {
                return BadRequest("user does not exist");
            }
            if (product == null)
            {
                return BadRequest("product does not exist");
            }
            return Ok(obj);
        }
    }
}
