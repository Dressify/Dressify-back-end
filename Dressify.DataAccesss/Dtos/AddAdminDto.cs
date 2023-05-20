using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class AddAdminDto
    {
        [Required]
        [MinLength(4)]
        public string AdminName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [JsonIgnore]
        public string? ProfilePic { get; set; }
        [JsonIgnore]
        public string? PublicId { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
