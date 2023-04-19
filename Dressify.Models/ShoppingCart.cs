﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class ShoppingCart
    {
        
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int? quantity { get; set; }
        public bool IsRent { get; set; }
    }
}
