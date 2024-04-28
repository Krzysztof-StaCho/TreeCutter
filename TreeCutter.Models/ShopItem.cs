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
    //Base class for Product and Bundle
    public abstract class ShopItem
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required][Range(1, 10000000)][Column(TypeName = "money")]
        public required decimal Price { get; set; }
        [Required]
        public required bool IsEnabled { get; set; } = true;
        [Required]
        public bool IsPromoted { get; set; } = true;

        [Required]
        public required int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
    }
}
