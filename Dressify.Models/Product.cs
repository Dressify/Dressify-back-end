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
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [DisplayName("Product Name"),MaxLength(50)]
        public string VendorId { get; set; }
        [ForeignKey("VendorId")]
        public ApplicationUser Vendor { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        [DisplayName("Number of sales")]
        public int NumberOfSales { get; set; }
        public float Sale { get; set; }
        public int Purchases { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? SuspendedUntil { get; set; }
        public string? Color { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? Type { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductQuestion>? Questions { get; set; }
        public List<ProductReport>? Reports { get; set; }
        public List<ProductAction>? ProdcutsActions { get; set; }
        public List<ShoppingCart>? Carts { get; set; }
        public List<OrderDetails>? OrdersDetails { get; set; }
        public List<ProductRate>?  ProductRates {get; set;}  



    }
}
