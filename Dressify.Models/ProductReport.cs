using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class ProductReport
    {
        [Key]
        public int ReportId { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public ApplicationUser Customer { get; set; }
        public string VendorId { get; set; }
        [ForeignKey("VendorId")]
        public ApplicationUser Vendor { get; set; }
        public string? AdminId { get; set; }
        [ForeignKey("AdminId")]
        public Admin? Admin { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [DefaultValue("unchecked")]
        public bool ReportStatus { get; set; }
        public string? Action { get; set; }

    }
}
