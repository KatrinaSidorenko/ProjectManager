using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using Core.Models;
using Core.Enums;
using UI.Interfaces;
using Core.Helpers;
using BLL;
using System.Threading.Channels;

namespace UI.ConsoleManagers
{
    public class UserUIManager : MainUIManager<User, IUserService>, IConsoleManager<User>
    {
        private Dictionary<string, object> _userPropertiesValues = new Dictionary<string, object>();
        public bool IsAdminExist;
        public bool IsAdminSessionNow;

        public UserUIManager(IUserService service) : base(service)
        {
            IsAdminExist = service.IsAdminExist().Result;
        }

        public async Task PerformOperations()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Edit user profile");
                Console.WriteLine("2. Exit");

                int input = InputValidator.IntegerValidator();

                if (input == 2)
                {
                    break;
                }
                else if (input == 1)
                {
                    Console.Clear();
                    await Edit();
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Invalid oparation number");
                }
            }

            Console.Clear();
        }

        public async Task PerformAdminOperations()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Change user status");
                Console.WriteLine("2. Exit");

                int input = InputValidator.IntegerValidator();

                if (input == 2)
                {
                    break;
                }
                else if (input == 1)
                {
                    Console.Clear();
                    await ChangeUserStatus();
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Invalid oparation number");
                }
            }

            Console.Clear();
        }

        public async Task ChangeUserStatus()
        {
            var tempallUsers = await Service.GetAll();
            var allUsers = tempallUsers.ToArray();
            if (allUsers.Length != 0)
            {
                for (var i = 0; i < allUsers.Length; i++)
                {
                    Console.WriteLine($"--------------------------- ({i + 1})");
                    await ShowObjects(new List<User> { allUsers[i] }, typeof(User));
                }

                int userIndex = InputValidator.IndexValidator(allUsers.Length);
                var user = allUsers[userIndex - 1];
                _userPropertiesValues.Clear();
                await GetUserStatus();
                user.UserStatus = (UserStatus)_userPropertiesValues["UserStatus"];
                await Service.Update(user.Id, user);

            }
            else
            {
                Console.WriteLine("No users");
                Console.ReadKey();
            }
        }
        public async Task ShowAllUsers()
        {
            var users = await GetAll();
            await ShowObjects(users.ToList(), typeof(User));
        }
        public async Task<Result<User>> Create()
        {
            _userPropertiesValues.Clear();
            await GetUserName();
            await GetUserEmail();
            await GetUserPassword();
            await GetUserStatus();

            try
            {
                var newUser = await CreateObject(_userPropertiesValues.Values.ToArray());             
                await Service.UserRegistration(newUser);

                return new Result<User>(false);
            }
            catch
            {
                Console.WriteLine("Failed to create user");
                Console.ReadKey();
                Console.Clear();
                return new Result<User>(true);
            }
            
        }

        private async Task GetUserName()
        {
            Console.WriteLine("Enter user name: ");
            string name = Console.ReadLine();

            if (await Service.CheckUserNameAvailability(name))
            {
                Console.WriteLine("You can't use this name");
                await GetUserName();
            }
            else
            {
                _userPropertiesValues.Add("Name", name);
            }
        }
        private async Task GetUserPassword()
        {
            Console.WriteLine("Enter user password: ");
            string password = PasswordsOparations.Hash(Console.ReadLine());
            _userPropertiesValues.Add("Password", password);
        }

        private async Task GetUserEmail()
        {
            Console.WriteLine("Enter user email: ");
            string email = Console.ReadLine();
            if(EmailSender.IsValidEmail(email))
            {
                _userPropertiesValues.Add("Email", email);
            }
            else
            {
                Console.WriteLine("Invalid email input format");
                await ReinvokeMethodHelper(GetUserEmail, WriteLine);
            }           
        }

        private async Task WriteLine()
        {
            Console.WriteLine();
        }

        private async Task GetUserStatus()
        {
            if(IsAdminExist)
            {
                Console.WriteLine("Available user statuses: ");

                foreach (var userStatus in Enum.GetValues(typeof(UserStatus)))
                {
                    if((UserStatus)userStatus != UserStatus.Admin)
                    {
                        Console.WriteLine($"\t{userStatus}");
                    }                   
                }

                Console.Write("Enter user status: ");

                UserStatus result;
                string status = Console.ReadLine();

                if (Enum.TryParse(status, out result))
                {
                    _userPropertiesValues.Add("UserStatus", result);
                }
                else
                {
                    await ReinvokeMethodHelper(GetUserStatus, WriteLine);
                    Console.ReadKey();
                }
            }
            else
            {
                _userPropertiesValues.Add("UserStatus", UserStatus.Admin);
                IsAdminSessionNow = true;
                IsAdminExist = true;
            }          
        }

        public async Task<Result<User>> Authentificate()
        {
            Console.WriteLine("Enter user name: ");
            string name = Console.ReadLine();
                   
            try
            {
                if (await Service.CheckUserNameAvailability(name))
                {
                    var user = await Service.GetByPredicate(user => user.Name == name);
                    if (user.UserStatus.Equals(UserStatus.Admin))
                    {
                        IsAdminSessionNow = true;
                    }
                    _userPropertiesValues.Clear();
                    _userPropertiesValues.Add("Name", name);
                    await GetUserEmail();
                    await GetUserPassword();
                    await Service.Authenticate((string)_userPropertiesValues["Name"], (string)_userPropertiesValues["Password"], (string)_userPropertiesValues["Email"]);

                    return new Result<User>(false);
                }
                else
                {
                    Console.WriteLine("This user doesn't exist");
                    Console.ReadKey();
                    Console.Clear();
                    return new Result<User>(true);
                }                
            }
            catch
            {
                Console.WriteLine("Fail to authetificate");
                Console.ReadKey();
                Console.Clear();
                return new Result<User>(true);
            }
            
        }

        public async Task Edit()
        {
            var editDictionary = new Dictionary<int, Func<Task>>()
            {
                {1, UpdateUserName},
                {2, UpdateUserPassword},
                {3, UpdateUserEmail},
                {4, UpdateUserStatus }
            };

            Console.WriteLine("Oparation for user profile changes:");
            Console.WriteLine("1. Change user name");
            Console.WriteLine("2. Change user password");
            Console.WriteLine("3. Change user email");
            Console.WriteLine("4. Change user status");

            int opartionNumber = InputValidator.IntegerValidator();

            if (editDictionary.ContainsKey(opartionNumber))
            {
                await editDictionary[opartionNumber].Invoke();
            }
            else
            {
                Console.WriteLine("Invalid oparation number");
            }
        }

        private async Task UpdateUserName()
        {
            try
            {
                Console.WriteLine();
                _userPropertiesValues.Clear();
                Console.WriteLine("New name operation".ToUpper());
                await GetUserName();
                await Service.UpdateUserName((string)_userPropertiesValues["Name"]);
            }
            catch
            {
                Console.WriteLine("Failed to reach user ");
            }
        }

        private async Task UpdateUserPassword()
        {
            try
            {
                Console.WriteLine();
                _userPropertiesValues.Clear ();
                Console.WriteLine("New password operation".ToUpper());
                await GetUserPassword();
                await Service.UpdatePassword((string)_userPropertiesValues["Password"]);
            }
            catch
            {
                Console.WriteLine("Fail to update password");
            }
        }

        private async Task UpdateUserEmail()
        {
            try
            {
                Console.WriteLine();
                _userPropertiesValues.Clear();
                Console.WriteLine("New email operation".ToUpper());
                await GetUserEmail();
                await Service.UpdateEmail((string)_userPropertiesValues["Email"]);
            }
            catch
            {
                Console.WriteLine("Fail to update email");
            }
        }

        private async Task UpdateUserStatus()
        {
            try
            {
                Console.WriteLine();
                _userPropertiesValues.Clear();
                Console.WriteLine("New user status operation".ToUpper());
                await GetUserStatus();
                await Service.UpdateUserStatus((UserStatus)_userPropertiesValues["UserStatus"]);
            }
            catch 
            {
                Console.WriteLine("Fail to update user status");
            }

        }

        public async Task ShowAllStakeHolders()
        {
            var sHolders = await Service.GetStakeHolders();

            var stakeHolders = sHolders.ToList();
            if( stakeHolders.Count > 0 )
            {
                await ShowObjects(stakeHolders, typeof(User));
            }
            else
            {
                Console.WriteLine("No StakeHolders");
            }
            
        }
    }
}
