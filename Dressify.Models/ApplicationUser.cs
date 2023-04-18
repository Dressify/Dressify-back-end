using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Dressify.Models
{
    public class ApplicationUser: IdentityUser 
    {

        public string? ProfilePic { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public List<WishList>? WishesLists { get; set; }

        public List<ProductQuestion>? QuestionsAsked { get; set; }


        //Vendor props
        public string? storeName { get; set; }
        public int? nId  { get; set; }
        public bool? isSuspended  { get; set; }

        public List<Product>? Products { get; set; }
        public List<ProductQuestion>? QuestionsAnswered { get; set; }
        public List<ProductReport>? Reports { get; set; }


    }
}
    