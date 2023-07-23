using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserAuthenticationService
    {
        Task<Result<LoginViewModel>> LoginAsync(LoginViewModel loginViewModel);
        Task<Result<AppUser>> RegisterAsync(RegisterViewModel registerViewModel);
        Task LogoutAsync();
        Task<List<AppUser>> GetUsersForTaskCompliting();
        Task<Result<AppUser>> GetUserById(string userId);
        Task<bool> IsEmailConfirmed(AppUser user);
    }
}
