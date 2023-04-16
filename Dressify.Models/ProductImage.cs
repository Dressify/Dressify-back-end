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
    public class ProductImage
    {
        [Key]
        public int ImageID { get; set; }
        public int ProductId { get; set; }
        public string? PublicId { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageExtension { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
