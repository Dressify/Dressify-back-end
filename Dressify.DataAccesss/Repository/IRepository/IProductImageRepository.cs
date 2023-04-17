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
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        Task<CreatePhotoDto> AddPhoto(IFormFile file);
    }
}
