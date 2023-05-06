using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Utility
{
    public static class SD
    {   //Roles
        public const string Role_Customer = "Customer";
        public const string Role_Vendor = "Vendor";
        public const string Role_Sales = "Sales";
        public const string Role_Admin = "Admin";
        public const string Role_SuperAdmin = "SuperAdmin";


        //order by
        public const string Ascending = "ASC";
        public const string Descending = "DESC";

        //Order  and Order Details Status
        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_Delivered = "Delivered";
        public const string Status_Refunded = "Rejected";
        //only for Order 
        public const string Status_Cancelled = "Cancelled";

        //Payment Methods 
        public const string PaymentMethod_Cash = "Cash";
        public const string PaymentMethod_Credit = "Credit";

    }
}
