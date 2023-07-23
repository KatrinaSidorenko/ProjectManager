using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagerNew.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
       
		public AccountController(IUserAuthenticationService userAuthenticationService,
            UserManager<AppUser> userManager,
            IEmailService emailService)
        {
            _userAuthenticationService = userAuthenticationService;
            _userManager = userManager;
            _emailService = emailService;
		}
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) 
                return View(loginViewModel);
                      
            var user = await _userManager.FindByNameAsync(loginViewModel.EmailAddress);
            if (user != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    TempData["Error"] = "You have not confirmed your email";
                    return View(loginViewModel);
                }
            }

            var loginResult = await _userAuthenticationService.LoginAsync(loginViewModel);
            if (!loginResult.IsSuccessful)
            {
                TempData["Error"] = loginResult.Message;

                return View(loginViewModel);
            }

            return RedirectToAction("Details", "Project");
        }
        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) 
                return View(registerViewModel);

            var registerResult = await _userAuthenticationService.RegisterAsync(registerViewModel);

            if(!registerResult.IsSuccessful)
            {
                TempData["Error"] = registerResult.Message;

                return View(registerViewModel);
            }
            else
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(registerResult.Data);
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = registerResult.Data.Id, code = code },
                    protocol: HttpContext.Request.Scheme);

                await _emailService.SendEmailAsync(registerViewModel.EmailAddress, "Confirm your account",
                    $"Confirm registration, follow the link: <a href='{callbackUrl}'>link</a>");

                TempData["Error"] = "To complete your registration, check your email and follow the link provided in the email";
                return View(registerViewModel);
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }
        public async Task<IActionResult> Logout()
        {
            await _userAuthenticationService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<AppUser, AppUserEditViewModel>());
            var mapper = new Mapper(config);
            var userVM = mapper.Map<AppUserEditViewModel>(user);
            userVM.EmailAddress = user.Email;

            return View(userVM);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(AppUserEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var roleConfirm = await _userManager.IsInRoleAsync(user, model.UserRole.ToString());
            if (!roleConfirm)
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRoleAsync(user, roles.FirstOrDefault());
                await _userManager.AddToRoleAsync(user, model.UserRole.ToString());
            }

            user.Email = model.EmailAddress;
            user.UserName = model.UserName;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }

    }
}
