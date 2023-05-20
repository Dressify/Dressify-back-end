using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class RegisterDto
    {
        [Required, StringLength(100)]
        public string FName { get; set; }
        [Required, StringLength(100)]
        public string LName { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(128)]
        public string Email { get; set; }

        [Required, StringLength(16)]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }


    }
}
