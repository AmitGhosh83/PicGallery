using System;
using System.Collections.Generic;
using System.Text;

namespace PicGallery.DataAccess.Models
{
    public class RoleUserModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
