using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class CustomerRateDto
    {
        public int? rate { get; set; }
        public string? RateComment { get; set; }
        public string? CustomerName { get; set; }
    }
}

