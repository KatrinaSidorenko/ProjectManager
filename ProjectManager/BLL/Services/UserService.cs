using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using DAL.Interfaces;
using Core.Helpers;
using Core.Enums;

namespace BLL.Services
{
    public class UserService : GenericService<User>, IUserService
    {
        private User _currentUser;
        public UserService(IRepository<User> repository) : base(repository)
        {
            _properties = typeof(User).GetProperties();
        }

        public async Task<User> GetCurrentUser()
        {
            return _currentUser;
        }

        public async Task<bool> Authenticate(string username, string password, string email)
        {
            var user = await GetByPredicate(user => user.Name == username && user.Password == password && user.Email == email);

            if (user != null)
            {
                _currentUser = user;
                return true;
            }

            return false;
        }

        public async Task<bool> CheckUserNameAvailability(string username)
        {
            var users = await GetAll();
            var usernames = users.Select(user => user.Name);
            
            return usernames.Contains(username);
        }

        public async Task DeleteUser(Guid id)
        {
            if(id != Guid.Empty)
            {
                await Delete(id);
            }
        }

        public async Task UpdateEmail(string email)
        {
            await GenericEdit(_currentUser.Id, email, "Email");
        }

        public async Task UpdatePassword(string password)
        {
            await GenericEdit(_currentUser.Id, password, "Password");
        }

        public async Task UserRegistration(User user)
        {
            if(user != null)
            {
                user.Id = Guid.NewGuid();
                _currentUser = user;
                await Add(user);
            }
        }

        public async Task UpdateUserName(string userName)
        {
            await GenericEdit(_currentUser.Id, userName, "Name");
        }

        public async Task<IList<User>> GetStakeHolders()
        {
            var users = await GetAll();
            var stakeHolders = new List<User>();
            foreach( var user in users)
            {
                if(user.UserStatus == UserStatus.StakeHolder)
                {
                    stakeHolders.Add(user);
                }
            }

            return stakeHolders;
        }

        public async Task UpdateUserStatus(UserStatus status)
        {
            await GenericEdit(_currentUser.Id, status, "UserStatus");
        }

        public async Task<bool> IsAdminExist()
        {
            var users = await GetAll();
            foreach(var user in users)
            {
                if(user.UserStatus == UserStatus.Admin)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
