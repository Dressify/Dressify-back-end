using Dressify.Utility;
using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.Models
{
    public class Order
    {

        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public ApplicationUser Customer { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public double? TotalPrice { get; set; }
        public string? payementMethod { get; set; }
        public DateTime? Date { get; set; }

        public DateTime? PaymentDate { get; set; }
        [DefaultValue(SD.Status_Pending)]
        public string OrderStatus { get; set; }
        public List<OrderDetails>? OrdersDetails { get; set; }
        public PayBill? PayBill { get; set; }

    }
}