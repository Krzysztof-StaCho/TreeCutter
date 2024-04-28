using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }
        [DisplayName("Phone Number")]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
