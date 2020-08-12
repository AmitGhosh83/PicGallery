using EmployeeManagement.DataAccess;
using EmployeeManagement.DataAccess.Models;
using KudVenvat1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using PicGallery.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KudVenvat1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, 
                                 SignInManager<ApplicationUser> signInManager,
                                 ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
            if (ModelState.IsValid)
            {
                // RegisterViewModel--> UserModel
                var userModel = new UserModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    City = model.City
                };

                //UserModel--> IdentityUser
                var user = new ApplicationUser
                {
                    UserName = userModel.Email,
                    Email = userModel.Email,
                    City = userModel.City
                };

                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    //Build the token and the Confirm Email Link
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { @userid = user.Id, token = token }, Request.Scheme);
                    _logger.Log(LogLevel.Warning, confirmationLink);

                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    ViewBag.ErrorTitle = "Registration Successful";
                    ViewBag.ErrorMessage = "Before you login, please confirm your email, We sent you a link for that";
                    return View("Error");

                    ////Instead of siginin in the user, we redirect then to Confirm Link
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> ConfirmEmail (string userId, string token)
        {
            if(userId==null || token==null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user==null)
            {
                ViewBag.ErrorMessage = $"The user id {userId} is invalid";
                return View("NotFound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = $"Email cannot be confirmed";
            return View("Error");

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            ViewData["ReturnUrl"] = Request.Query["returnUrl"];
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            // To ensure if local login fails, the external logins are still displayed/
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // LoginViewModel--> UserModel
                var user = new LoginUserModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                };

                var userModel = await _userManager.FindByEmailAsync(user.Email);
                if (userModel != null && !userModel.EmailConfirmed && (await _userManager.CheckPasswordAsync(userModel, model.Password)))
                {
                    ModelState.AddModelError("", "Email is not confirmed yet");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);


                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }


        #region External Logins
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginUserModel loginViewModel = new LoginUserModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = loginViewModel.ReturnUrl,
                ExternalLogins = loginViewModel.ExternalLogins
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", model);
            }

            //To get external login info from login provider
            var information = await _signInManager.GetExternalLoginInfoAsync();
            if (information == null)
            {
                ModelState.AddModelError(string.Empty, $"Error loading external login information");
                return View("Login", model);
            }

            // Get the email claim value
            var email = information.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Email is not confirmed yet");
                    return View("Login", model);
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(information.LoginProvider, information.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = information.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = information.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await _userManager.CreateAsync(user);
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(user, information);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {information.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on amitghosh83@gmail.com";

                return View("Error");
            }

        }

        #endregion
    }
}
