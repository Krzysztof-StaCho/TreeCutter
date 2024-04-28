using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class ManageProfileVM
    {
        [Required]
        public string Username { get; set; }
        [DisplayName("Phone number")]
        public string? PhoneNumber { get; set; }
        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }
    }
}
