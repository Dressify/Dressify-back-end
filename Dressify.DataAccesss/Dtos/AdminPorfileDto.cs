using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class AdminPorfileDto
    {
        public string? AdminId { get; set; }
        [Required]
        [MinLength(4)]
        public string AdminName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? ProfilePic { get; set; }
    }
}
