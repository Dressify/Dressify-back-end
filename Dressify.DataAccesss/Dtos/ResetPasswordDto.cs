using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class ResetPasswordDto
    {
        public string? Password { get; set; }
        [Compare("Password",ErrorMessage ="The password and confirmation Password do not match.")]
        public string ConfirmPassword  { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }



    }
}
