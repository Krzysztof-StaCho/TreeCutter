using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class OrderVM
    {
        public required OrderHeader OrderHeader { get; set; }
        public required IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
