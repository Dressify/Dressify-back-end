using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class ProductRatesDto
    {
        public List<CustomerRateDto>? customerRates;
        public int? Count { get; set; }
        public decimal? average { get; set; }
    }
}

