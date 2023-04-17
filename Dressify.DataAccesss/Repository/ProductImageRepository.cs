using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Helpers;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    internal class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public ProductImageRepository(ApplicationDbContext context, IOptions<CloudinarySettings> cloudinary) : base(context)
        {
            _context = context;
            _cloudinaryConfig = cloudinary;
        }

        //Add Photo To Cloudinary 
        public async Task<CreatePhotoDto> AddPhoto(IFormFile file)
        {
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.Key,
                _cloudinaryConfig.Value.Secret
                );
            _cloudinary = new Cloudinary(acc);
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            var CreatedPhoto = new CreatePhotoDto {
                Url = uploadResult.Url.ToString(),
                PublicId = uploadResult.PublicId
                };
            return CreatedPhoto;
        }

    }
}
