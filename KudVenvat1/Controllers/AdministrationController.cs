using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using PicGallery.DataAccess.Models;
using PicGallery.ViewModels;

namespace PicGallery.Controllers
{
  
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

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
                Claims = userClaims.Select(c=>c.Value).ToList()
                
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
    }
}
