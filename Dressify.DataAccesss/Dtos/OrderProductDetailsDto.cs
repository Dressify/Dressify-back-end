using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class OrderProductDetailsDto
    {
        public string img { get; set; }
        public double ProductPrice { get; set; }
        public int? Quantity { get; set; }
        public string ProductName { get; set; }
    }
}
