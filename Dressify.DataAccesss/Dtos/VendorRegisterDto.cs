using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class VendorRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required, StringLength(128),EmailAddress]
        public string Email { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        [Required]
        public string nId {get; set; } 
        [Required, StringLength(16)]
        public string Password { get; set; }
        public string Phone { get; set; }
        [Required]
        public string address { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(20)]
        public string StoreName { get; set; }

    }
}
