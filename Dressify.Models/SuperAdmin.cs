using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class SuperAdmin
    {
        [Key]
        public string SuperAdminId { get; set; }
        public string? UserName { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public SuperAdmin()
        {
            this.SuperAdminId = Guid.NewGuid().ToString();
        }
    }

    
}

