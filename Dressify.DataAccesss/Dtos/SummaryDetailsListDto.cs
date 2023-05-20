using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class SummaryDetailsListDto
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string  ProductName { get; set; }
    }
}