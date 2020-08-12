using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace EmployeeManagement.DataAccess.Models
{
    public class LoginUserModel
    {
        public LoginUserModel()
        {
            ExternalLogins = new List<Microsoft.AspNetCore.Authentication.AuthenticationScheme>();
        }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public IList<Microsoft.AspNetCore.Authentication.AuthenticationScheme> ExternalLogins { get; set; }
    }
}
