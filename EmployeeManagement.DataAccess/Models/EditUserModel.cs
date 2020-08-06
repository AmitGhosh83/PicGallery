using EmployeeManagement.DataAccess;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PicGallery.DataAccess.Models
{
    public class EditUserModel: UserModel
    {
        public EditUserModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        [Required]
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<string> Claims { get; set; }
        public List<string> Roles { get; set; }
    }
}
