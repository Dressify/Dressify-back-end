using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class CreateProductDto
    {
        public string VendorId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        [DisplayName("Number of sales")]
        public int NumberOfSales { get; set; }
        public float Sale { get; set; }
        public int Purchases { get; set; }
        public bool Rentable { get; set; }
        public bool Suspended { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Type { get; set; }
        public List<FileStream> Photos { get; set; }
    }
}
