using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Models;

namespace BLL.Interfaces
{
    public interface IUserService : IGenericService<User>
    {
        Task UserRegistration(User user);
        Task<bool> Authenticate(string username, string password, string email);
        Task UpdatePassword(string password);
        Task UpdateEmail(string email);
        Task UpdateUserName(string userName);
        Task UpdateUserStatus(UserStatus status);
        Task DeleteUser(Guid id);
        Task<bool> CheckUserNameAvailability(string username);
        Task<IList<User>> GetStakeHolders();
        Task<User> GetCurrentUser();
        Task<bool> IsAdminExist();
    }
}
