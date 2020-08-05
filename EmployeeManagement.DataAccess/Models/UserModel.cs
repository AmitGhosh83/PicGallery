using PicGallery.DataAccess.BusinessRule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeManagement.DataAccess
{
    public class UserModel
    {
        [Required]
        [EmailAddress]
        
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password doesnt match")]
        [ValidEmailDomainModel(allowedDomain:"gmail.com", ErrorMessage ="Allowing Gmail domain at this point")]
        public string ConfirmPassword { get; set; }
    }
}
