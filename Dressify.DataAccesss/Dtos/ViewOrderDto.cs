using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class ViewOrderDto
    {
        
        public int orderId { get; set; }
        public float TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime dateTime { get; set; }
        public string paymentMethod { get; set; }
        public int Quantity { get; set; }
        public List<OrderProductDetailsDto> ProductDetails { get; set; }
     }
}