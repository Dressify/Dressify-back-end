﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace dressify.Models
{
    public class ApplicationUser: IdentityUser
    {

        public string? ProfilePic { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age { get; set; }

        //Vendor props
        public string? storeName { get; set; }
        public int? nId  { get; set; }
        public bool? isSuspended  { get; set; }

    }
}
