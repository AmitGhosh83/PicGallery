using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

// To implement custom ManageAdminRolesAndClaimsRequirement we need a AuthorizationHandler whihc implements the same 

namespace PicGallery.Security
{
    public class CanEditOnlyOtherAdminRolesandClaimHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CanEditOnlyOtherAdminRolesandClaimHandler(IHttpContextAccessor  httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {


            var authFilterContext = httpContextAccessor.HttpContext;
            if(authFilterContext==null)
            {
                return Task.CompletedTask;
            }

           
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string adminIdBeingEdited = httpContextAccessor.HttpContext.Request.Query["userId"];

            if (context.User.IsInRole("Admin")&&
                context.User.HasClaim(c=>c.Type=="Edit Role" && c.Value=="true") &&
                adminIdBeingEdited.ToLower()!= loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
