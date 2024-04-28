using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models
{
    public class Page
    {
        public int Id { get; set; }
        [Required][MaxLength(30)]
        public required string Title { get; set; }
        [Required][Column(TypeName = "nvarchar(MAX)")]
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        [DisplayName("Display Order")]
        public int Order { get; set; }
    }
}
