using Dressify.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class OrderDetails
    {
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        public bool IsRent { get; set; }
        [DefaultValue(SD.Status_Pending)]
        public string Status { get; set; }
        public string? ProductName { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? VendorId{ get; set;}
        [ForeignKey("VendorId")]
        public ApplicationUser Vendor { get; set; }
    }
}