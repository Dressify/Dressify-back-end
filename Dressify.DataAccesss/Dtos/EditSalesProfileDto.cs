using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class EditSalesProfileDto
    {
        [Required]
        public string SalesId { get; set; }
        [Required]
        public string SalesName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        [Required]
        public string NId { get; set; }

    }
}
