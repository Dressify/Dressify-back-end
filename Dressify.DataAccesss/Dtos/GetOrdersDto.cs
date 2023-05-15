using Dressify.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class GetOrdersDto
    {
        //int pageNumber, int pageSize, double? minPrice, double? maxPrice, string? gender, string? category
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
