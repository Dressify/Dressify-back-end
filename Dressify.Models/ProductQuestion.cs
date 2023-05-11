using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Dressify.Models
{
    public class ProductQuestion
    {
        [Key]
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }
        public DateTime QuestionDate { get; set; }=DateTime.Now;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public string? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public ApplicationUser? Vendor { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public ApplicationUser Customer { get; set; }

    }
}
