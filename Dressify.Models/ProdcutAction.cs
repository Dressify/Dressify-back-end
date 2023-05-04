using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class ProdcutAction
    {
        public string AdminId { get; set; }
        [ForeignKey("AdminId")]
        public Admin Admin { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public string VendorId { get; set; }
        [ForeignKey("VendorId")]
        public ApplicationUser Vendor { get; set; }
        public string? action { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime SuspendedUntil { get; set; }
    }
}
