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
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public List<SummaryDetailsListDto> detailsList { get; set; }
    }
}