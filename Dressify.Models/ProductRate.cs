using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class ProductRate

    {

        public string CustomerId { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("CustomerId")]
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int? rate { get; set; }
        public string? RateComment { get; set; }
        public bool IsPurchased { get; set; }
        public DateTime? Date { get; set; }


    }
}
