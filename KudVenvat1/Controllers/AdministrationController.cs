using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
