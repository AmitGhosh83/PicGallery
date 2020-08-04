using KudVenvat1.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KudVenvat1.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid Email Format")]
        [Display(Name = "EmailID")]
        public string Email { get; set; }


        [Display(Name = "Dept")]
        [Required]
        public Dept? Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}
