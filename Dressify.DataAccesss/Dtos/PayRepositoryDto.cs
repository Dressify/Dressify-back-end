using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class PayRepositoryDto
    {

        public string? paymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Status { get; set;}
    }
}