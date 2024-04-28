using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class BundleVM
    {
        [Required]
        public required Bundle Model { get; set; }
        public required List<int> ProductsId { get; set; }
    }
}
