using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class VendorProfileDto
    {
        public string? VendorID { get; set; }
        public string? Address { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? imgUrl { get; set; }
        public string? PhoneNumber { get; set;}
        public string? StoreName { get; set;}
        public bool? IsSuspended { get; set;}
        public DateTime? SuspednedUntil { get; set; }
        public string? NId { get; set; }

    }
}
