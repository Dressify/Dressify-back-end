using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class CreatePhotoDto
    {
        public string Url { get; set; }
        public string PublictId { get; set; }
    }
}

