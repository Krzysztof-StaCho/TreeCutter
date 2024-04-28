using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class RoleManagmentVM
    {
        public required ApplicationUser ApplicationUser { get; set; }
        public required IEnumerable<SelectListItem> RoleList { get; set; }
        public required IEnumerable<SelectListItem> CompanyList { get; set; }

        [Required]
        public string? NewRole { get; set; }
    }
}
