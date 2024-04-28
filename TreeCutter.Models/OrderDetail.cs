using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int OrderHeaderId { get; set; }
        [ForeignKey(nameof(OrderHeaderId))][ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ShopItemId { get; set; }
        [ForeignKey(nameof(ShopItemId))]
        [ValidateNever]
        public ShopItem ShopItem { get; set; }

        public int Count { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
    }
}
