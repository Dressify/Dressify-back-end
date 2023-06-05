using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Utility
{
    public static class SD
    { 
        //Roles
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
        public const string Status_Confirmed = "Confirmed";
        public const string Status_Delivered = "Delivered";
        public const string Status_Shipped = "Shipped";

        //only for Order 
        public const string Status_Cancelled = "Cancelled";

        //Payment Methods 
        public const string PaymentMethod_Cash = "Cash";
        public const string PaymentMethod_Credit = "Credit";

        //Payment Status
        public const string PaymentStatus_Succeded = "succeeded";
        public const string PaymentStatus_Failed = "failed";

        //Product Actions
        public const string Action_Ignore = "ignore";
        public const string Action_SuspendProduct = "suspendProduct";
        public const string Action_SuspendVendor = "suspendVendor";

        //Sales info
        public const string ImgUrl = "https://res.cloudinary.com/ddsavy6nu/image/upload/v1684009222/xgm4z70exhppblassiaf_g38jon.png";
        public const string PublicId = "xgm4z70exhppblassiaf_g38jon";
        public const string Address = "FCI-Helwan";
        public const string Phone = "01028542932";
        public const string StoreName = "Dressify";

        //AI URLS
        public const string AIUrl = "http://127.0.0.1:5000/";
        public const string AIPredict = "predict";
        public const string AIAddProduct = "add_product";


    }
}
