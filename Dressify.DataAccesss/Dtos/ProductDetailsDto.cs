using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class ProductDetailsDto
    {
        public Product Product { get; set; }
        public double AverageRate { get; set; }
        public int? Quantity { get; set; }

    }
}
