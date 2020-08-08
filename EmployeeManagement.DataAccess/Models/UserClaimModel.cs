using System;
using System.Collections.Generic;
using System.Text;

namespace PicGallery.DataAccess.Models
{
    public class UserClaimModel
    {
        public UserClaimModel()
        {
            Claims = new List<UserClaim>();
        }
        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
