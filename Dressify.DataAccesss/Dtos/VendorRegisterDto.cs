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
        public string UserName { get; set; }
        [Required, StringLength(128)]
        public string Email { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public int nId {get; set; } 
        [Required, StringLength(16)]
        public string Password { get; set; }
        public string Phone { get; set; }
        public string address { get; set; }
    }
}
