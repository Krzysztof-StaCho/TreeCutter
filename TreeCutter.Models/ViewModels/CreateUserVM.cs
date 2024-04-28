using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class CreateUserVM
    {
        public required RegisterVM Input { get; set; }
        [ValidateNever]
        public required IEnumerable<SelectListItem> RoleList { get; set; }
        [ValidateNever]
        public required IEnumerable<SelectListItem> CompanyList { get; set; }

        [Required]
        public string NewRole { get; set; }
        public int? CompanyId { get; set; }
    }
}
