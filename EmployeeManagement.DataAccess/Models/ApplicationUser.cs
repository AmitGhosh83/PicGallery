using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PicGallery.DataAccess.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string  City { get; set; }
    }
}
