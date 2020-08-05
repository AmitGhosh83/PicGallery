using EmployeeManagement.DataAccess;
using EmployeeManagement.DataAccess.Models;
using KudVenvat1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KudVenvat1.Controllers
{
    public class AccountController :Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //[AcceptVerbs("Get","Post")]
        //[AllowAnonymous]
        //public  async Task<IActionResult> IsEmailAlreadyUsed(string inputEmail)
        //{
        //    var user = await _userManager.FindByEmailAsync(inputEmail);
        //    if (user == null)
        //    {
        //        return Json(true);
        //    }
        //    else
        //    {
        //        return Json($"Email {inputEmail} is already in use");
        //    }
        //}

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                // RegisterViewModel--> UserModel
                var userModel = new UserModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                };

                //UserModel--> IdentityUser
                var user = new IdentityUser
                {
                    UserName = userModel.Email,
                    Email = userModel.Email
                };

                var result= await _userManager.CreateAsync(user, userModel.Password);
                if(result.Succeeded)
                {
                   await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewData["ReturnUrl"] = Request.Query["returnUrl"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // LoginViewModel--> UserModel
                var user = new LoginUserModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                };

                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);


                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }
    }
}
