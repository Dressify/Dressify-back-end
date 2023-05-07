using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class TokenRequestDto
    {
        [Required]
        public string stringLogin { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
