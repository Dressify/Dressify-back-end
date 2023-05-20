using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class RateDto
    {
        public int ProductId { get; set; }
        public int? rate { get; set; }
        public string? RateComment { get; set; }
    }
}

