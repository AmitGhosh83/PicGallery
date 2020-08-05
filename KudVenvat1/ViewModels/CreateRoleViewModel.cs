using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PicGallery.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage ="Role Name is required")]
        public string RoleName { get; set; }
    }
}
