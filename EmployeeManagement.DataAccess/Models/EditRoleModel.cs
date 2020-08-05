using System;
using System.Collections.Generic;
using System.Text;

namespace PicGallery.DataAccess.Models
{
    public class EditRoleModel: CreateRoleModel
    {
        public EditRoleModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }
        public List<string> Users { get; set; }
    }
}
