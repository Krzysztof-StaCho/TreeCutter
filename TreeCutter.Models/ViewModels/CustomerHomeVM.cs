using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class CustomerHomeVM
    {
        public required IEnumerable<Product> ProductList { get; set; }
        public required IEnumerable<Bundle> BundleList { get; set; }
    }
}
