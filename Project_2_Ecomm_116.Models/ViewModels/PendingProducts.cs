using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2_Ecomm_116.Models.ViewModels
{
    public class PendingProducts
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public IEnumerable<OrderHeader> OrderStatus { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        public OrderDetail OrderDetail { get; set; }
        public OrderHeader OrderHeader { get; set; }      
       
    }
}
