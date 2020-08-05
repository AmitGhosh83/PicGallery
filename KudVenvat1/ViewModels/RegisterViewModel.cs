using Microsoft.AspNetCore.Mvc;
using PicGallery.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KudVenvat1.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        //[Remote(action: "IsEmailAlreadyUsed", controller: "Account")]
        [ValidEmailDomain(allowedDomain:"gmail.com", ErrorMessage ="Only allowing gmail.com users")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Password and Confirm Password doesnt match")]
        public string ConfirmPassword { get; set; }
    }
}
