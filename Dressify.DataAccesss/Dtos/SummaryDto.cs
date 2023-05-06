using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class SummaryDto
    {
        public Order Order { get; set; }
        public IEnumerable<ShoppingCart> ListCart { get; set; }
    }
}