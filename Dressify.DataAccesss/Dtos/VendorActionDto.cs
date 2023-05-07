using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class VendorActionDto
    {
        [Required]
        public string VendorId { get; set; }
        [Required]
        public string? Reasson { get; set; }
        public string? SuspendedUntil { get; set; }
    }
}
