using BLL.Interfaces;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace BLL.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAuthenticationService(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<Result<LoginViewModel>> LoginAsync(LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return new Result<LoginViewModel>(true, "Log in is successfully");
                    }
                }
                return new Result<LoginViewModel>(false, "Wrong credentials. Please try again");
            }

            return new Result<LoginViewModel>(false, "Wrong credentials. Please try again");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Result<AppUser>> RegisterAsync(RegisterViewModel registerViewModel)
        {
            var userExist = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

            if (userExist != null)
            {
                return new Result<AppUser>(false, "This email address is already in use");
            }

            AppUser newUser = new AppUser()
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.EmailAddress
            };

            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (!result.Succeeded)
            {
                return new Result<AppUser>(false, "User creation failed");
            }

            //role managment
            if (await _roleManager.RoleExistsAsync(registerViewModel.UserRole.ToString()))
                await _userManager.AddToRoleAsync(newUser, registerViewModel.UserRole.ToString());


            return new Result<AppUser>(true, newUser);
        }

        public async Task<List<AppUser>> GetUsersForTaskCompliting()
        {
            var allUsers = _userManager.Users.ToList();

            var possibleExecutors= new List<AppUser>();

            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                if (!userRoles.Contains(UserRoles.Admin.ToString()))
                {
                    possibleExecutors.Add(user);
                }
            }

            return possibleExecutors;
        }

        public async Task<Result<AppUser>> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new Result<AppUser>(false);

            return new Result<AppUser>(true, user);
        }

        public async Task<bool> IsEmailConfirmed(AppUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }
    }
}
