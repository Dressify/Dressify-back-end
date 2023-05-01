using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dressify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public SuperAdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody]AddAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _unitOfWork.SuperAdmin.CreateAdminAsync(dto);
            return Ok(result);
        }
    }
}
