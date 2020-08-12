using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PicGallery.DataAccess.Models
{
    public class ForgotPasswordUserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
