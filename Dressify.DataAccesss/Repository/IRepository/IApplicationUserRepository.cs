using Dressify.DataAccess.Dtos;
using Dressify.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task<AuthDto> CustomerRegisterAsync(CustRegisterDto dto);
        //Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto> GetTokenAsync(TokenRequestDto dto);
        Task<ApplicationUser> GetUserAsync(string userId);
        Task<string> GetRoleAsync(ApplicationUser user);
        Task<CreatePhotoDto> AddPhoto(IFormFile file);
        Task<string> DeletePhoto(string publicId);
        Task<AuthDto> VendorRegisterAsync(VendorRegisterDto dto);

    }
}
