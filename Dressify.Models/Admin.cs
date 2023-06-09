﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class Admin
    {
        [Key]
        public string AdminId { get; set; }
        public string AdminName { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string Email { get; set; }
        public string ProfilePic { get; set; }
        public string PublicId { get; set; }
        public List<ProductReport>? Reports { get; set; }
        public List<Penalty>? Penalties { get; set; }
        public List<ProductAction>? ProdcutsActions { get; set; }




        public Admin()
        {
            this.AdminId = Guid.NewGuid().ToString();
        }
    }
}
