using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TreeCutter.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required][MaxLength(30)][DisplayName("Category Name")]
        public required string Name { get; set; }
        [Required][Range(0, 100, ErrorMessage = "Display Order must be between 1-100")]
        public required int DisplayOrder { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; } = new List<Product>();
        [JsonIgnore]
        public ICollection<Bundle> Bundles { get; } = new List<Bundle>();
    }
}
