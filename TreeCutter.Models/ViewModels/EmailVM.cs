using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class EmailVM
    {
        [EmailAddress]
        public required string Email { get; set; }

        [EmailAddress]
        public string? NewEmail { get; set; }
    }
}
