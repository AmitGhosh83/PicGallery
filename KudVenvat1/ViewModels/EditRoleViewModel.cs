using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicGallery.ViewModels
{
    public class EditRoleViewModel: CreateRoleViewModel
    {
        public EditRoleViewModel()
        {
             Users = new List<string>();
        }
        public string Id { get; set; }
        public List<string> Users { get; set; }
    }
}
