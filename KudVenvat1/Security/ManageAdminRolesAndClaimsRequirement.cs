using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Admin can add/update/delete roles and claims of other admins but not self
//For this we need access to the userid in the Edit User Query String and so the need of custom authorization requirement

namespace PicGallery.Security
{
    public class ManageAdminRolesAndClaimsRequirement: IAuthorizationRequirement
    {
    }
}
