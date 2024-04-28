using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TreeCutter.Models
{
    public class Bundle : ShopItem
    {
        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
