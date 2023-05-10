using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Dressify.Models
{
    public class ApplicationUser: IdentityUser 
    {

        public string? ProfilePic { get; set; }
        public string? PublicId { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age => CalculateAge();
        public string? FName { get; set; }
        public string? LName { get; set; }
        private int? CalculateAge()
        {
            var today = DateTime.Today;
            if (DOB != null)
            {
                var age = today.Year - DOB.Value.Year;

                if (DOB > today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
            return null;
        }
        public List<WishList>? WishesLists { get; set; }

        public List<ProductQuestion>? QuestionsAsked { get; set; }
        public List<ProductReport>? Reports { get; set; }
        public List<ShoppingCart>? Carts { get; set; }
        public List<Order>? Orders { get; set; }
        public List<OrderDetails>? OrdersDetails { get; set; }

        //Vendor props
        public string? StoreName { get; set; }
        public int? NId  { get; set; }
        public bool IsSuspended  { get; set; }=false;
        public DateTime? SuspendedUntil { get; set; }


        public List<Product>? Products { get; set; }
        public List<ProductQuestion>? QuestionsAnswered { get; set; }
        public List<Penalty>? Penalties { get; set; }




    }
}
    