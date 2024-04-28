using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        [ForeignKey("ItemId")][ValidateNever]
        public ShopItem Item { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")][ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public decimal Price
        {
            get
            {
                if (Item == null) return 0;
                return Item.Price * Count;
            }
        }
    }
}
