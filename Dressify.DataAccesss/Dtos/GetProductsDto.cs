using Dressify.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class GetProductsDto
    {
        //int pageNumber, int pageSize, double? minPrice, double? maxPrice, string? gender, string? category
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string? Gender { get; set; }
        public string? Category { get; set; }
        public bool IsFilter { get; set; } 
    }
}
