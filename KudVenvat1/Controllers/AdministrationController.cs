using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using PicGallery.DataAccess.Models;
using PicGallery.Models;
using PicGallery.ViewModels;

namespace PicGallery.Controllers
{
  
    [Authorize(Roles ="Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager, 
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }
        #region User
        [HttpGet]
        public IActionResult ListUsers()
        {
            var model = _userManager.Users;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user==null)
            {
                ViewBag.ErrorMessage = $"User with {id} not found";
                return View("NotFound");
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var userModel = new EditUserModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Roles = userRoles.ToList(),
                Claims = userClaims.Select(c => c.Type+ " : "+ c.Value).ToList()
                
            };
            var model = new EditUserViewModel
            {
                Id = userModel.Id,
                Email = userModel.Email,
                UserName = userModel.UserName,
                City = user.City,
                Roles = userModel.Roles,
                Claims = userModel.Claims
            };
                       

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with {model.Id} not found";
                return View("NotFound");
            }
            else
            {
                var userModel = new EditUserModel
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    City = model.City,
                };

                user.Email = userModel.Email;
                user.UserName = userModel.UserName;
                user.City = userModel.City;

                var result = await _userManager.UpdateAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Administration");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with {id} not found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Administration");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListUsers");
                }
            }
        }

        #endregion

        #region Role
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // CreateRoleViewModel --> CreateRoleModel
                var roleModel = new CreateRoleModel { Role = model.RoleName };
                //CreateRoleModel--> IdentityRole
                var role = new IdentityRole { Name = roleModel.Role };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
       
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await _roleManager.FindByIdAsync(id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Roleid :{id} cannot be found";
                return View("NotFound");
            }
            //RoleIdentity --> EditRoleModel
            var roleModel = new EditRoleModel
            {
                Id = result.Id,
                Role = result.Name,
            };
            foreach (var user in await _userManager.GetUsersInRoleAsync(result.Name))
            {
                if (await _userManager.IsInRoleAsync(user, result.Name))
                {
                    roleModel.Users.Add(user.UserName);
                }
            }

            //EditRoleModel--> EditViewRoleModel
            var model = new EditRoleViewModel
            {
                Id = roleModel.Id,
                RoleName = roleModel.Role,

            };
            foreach (var user in roleModel.Users)
            {
                model.Users.Add(user);
            }
            return View(model);
        }

        [HttpPost]
       
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    //EditRoleViewModel--> EditRoleModel
                    var editRoleModel = new EditRoleModel
                    {
                        Id = model.Id,
                        Role = model.RoleName,
                    };
                    foreach (var item in model.Users)
                    {
                        editRoleModel.Users.Add(item);
                    }

                    //EditRoleModel --> RoleIdentity
                    role.Name = editRoleModel.Role;

                    //Update the RoleIdentity

                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"User with {id} not found";
                return View("NotFound");
            }
            else
            {
                //throw new Exception();
                try
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles", "Administration");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("ListRoles");
                    }
                }
                catch(DbUpdateException ex)
                {
                    _logger.LogError($"Error deleting role {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} has associated users, " +
                        $" you need to delete users associated before deleing the role";
                    return View("Error");
                }
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var result = await _roleManager.FindByIdAsync(roleId);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Roleid :{roleId} cannot be found";
                return View("NotFound");
            }

            var users = await _userManager.Users.ToListAsync();

            //UserRole --> UserRoleModel
            var model = new List<UserRoleModel>();
            foreach (var user in  users)
            {
                var userRoleModel = new UserRoleModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                //
                if (await _userManager.IsInRoleAsync(user, result.Name))
                {
                    userRoleModel.IsSelected = true;
                }
                else
                {
                    userRoleModel.IsSelected = false;
                }

                //userRoleModel.IsSelected = (await _userManager.IsInRoleAsync(user, result.Name)) ? true : false;
                model.Add(userRoleModel);
            }
            
            //UserRoleModel --> UserRoleViewModel
            var userRoleViewModel = new List<UserRoleViewModel>();
            foreach (var user in model)
            {
                var userrole = new UserRoleViewModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    IsSelected = user.IsSelected
                };

                userRoleViewModel.Add(userrole);
            }
            return View(userRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole (List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Roleid :{roleId} cannot be found";
                return View("NotFound");
            }
            var userRolesModel = new List<UserRoleModel>();
            for (int i = 0; i < model.Count; i++)
            {
                var userRoleModel = new UserRoleModel
                {
                    UserId = model[i].UserId,
                    UserName = model[i].UserName,
                    IsSelected = model[i].IsSelected
                };
                userRolesModel.Add(userRoleModel);
            }

            for (int i = 0; i < userRolesModel.Count; i++)
            {
               var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if(model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                  result=  await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (! model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < model.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = role.Id });
                }
            }

            return RedirectToAction("EditRole", new { Id= role.Id});
        }


        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRolesInUser(string userId)
        {
            ViewBag.userId = userId;
            var result = await _userManager.FindByIdAsync(userId);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"User with id :{userId} cannot be found";
                return View("NotFound");
            }
            //This to ensure we wait for the 1st database call
            //to finish, before we make another call to scoped identity
            var roles = await _roleManager.Roles.ToListAsync();

            //RoleUser --> RoleUserModel
            var model = new List<RoleUserModel>();
            foreach (var role in roles)
            {
                var roleUserModel = new RoleUserModel
                {
                    RoleId = role.Id,
                    RoleName= role.Name
                };
                //
                if (await _userManager.IsInRoleAsync(result,role.Name))
                {
                    roleUserModel.IsSelected = true;
                }
                else
                {
                    roleUserModel.IsSelected = false;
                }

                //userRoleModel.IsSelected = (await _userManager.IsInRoleAsync(user, result.Name)) ? true : false;
                model.Add(roleUserModel);
            }

            //UserRoleModel --> UserRoleViewModel
            var roleUserViewModel = new List<RoleUserViewModel>();
            foreach (var role in model)
            {
                var roleUser = new RoleUserViewModel
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    IsSelected = role.IsSelected
                };

                roleUserViewModel.Add(roleUser);
            }
            return View(roleUserViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRolesInUser(List<RoleUserViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id :{userId} cannot be found";
                return View("NotFound");
            }
            var roleUsersModel = new List<RoleUserModel>();
            for (int i = 0; i < model.Count; i++)
            {
                var roleUserModel = new RoleUserModel
                {
                    RoleId = model[i].RoleId,
                    RoleName = model[i].RoleName,
                    IsSelected = model[i].IsSelected
                };
                roleUsersModel.Add(roleUserModel);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user,
                roleUsersModel.Where(x => x.IsSelected).Select(x => x.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }

        #endregion

        #region Claim
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user==null)
            {
                ViewBag.ErrorMessage = $"User with {userId} can't be found";
                return View("NotFound");
            }
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            //ClaimIdentity--> UserClaimModel
            var dataModel = new UserClaimModel
            {
                UserId = userId,
            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                var userClaim = new DataAccess.Models.UserClaim
                {
                    ClaimType = claim.Type,
                };
                if(existingUserClaims.Any(x=>x.Type==claim.Type && x.Value=="true"))
                {
                    userClaim.IsSelected = true;
                }
                dataModel.Claims.Add(userClaim);
            }

            //UserClaimModel --> UserClaimViewModel

            var model = new UserClaimViewModel
            {
                UserId = dataModel.UserId,
            };
            foreach (var item in dataModel.Claims)
            {
                var userclaim = new PicGallery.ViewModels.UserClaim
                {
                    ClaimType = item.ClaimType,
                    IsSelected = item.IsSelected
                    
                };
                model.Claims.Add(userclaim);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaims(UserClaimViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }
            //Get users existing claims and delete them

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Canot remove existing user claims");
                return View(model);
            }
            //UserClaimViewModel--> UserClaimModel
            var userModel = new UserClaimModel
            {
                UserId = model.UserId,
            };
            foreach (var item in model.Claims.Where(x=>x.IsSelected))
            {
                var userclaim = new PicGallery.DataAccess.Models.UserClaim
                {
                     ClaimType = item.ClaimType,
                     IsSelected = item.IsSelected
                };
                userModel.Claims.Add(userclaim);
            }

            result = await _userManager.AddClaimsAsync(user,
            userModel.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected? "true":"false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id= model.UserId});
        }
        #endregion
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
