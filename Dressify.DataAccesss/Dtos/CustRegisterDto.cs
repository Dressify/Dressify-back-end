using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class CustRegisterDto
    {
        public string Username { get; set; }
        [Required, StringLength(128)]
        public string Email { get; set; }

        [Required, StringLength(16)]
        public string Password { get; set; }
    }
}
