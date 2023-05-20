using Dressify.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class AddToCartDto
    {
        public string? CustomerId { get; set; }
        public int ProductId { get; set; }
        public int? quantity { get; set; }
    }
}
