using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class CustomerProfileDto
    {
        public string? Address { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
    }
}
