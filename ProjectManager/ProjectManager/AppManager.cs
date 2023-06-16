using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.ConsoleManagers;
using Core.Helpers;

namespace UI
{
    public class AppManager
    {
        private readonly UserUIManager _userConsoleManager;
        private readonly TaskUIManager _taskConsoleManager;
        private readonly ProjectUIManager _projectConsoleManager;
        

        public AppManager(UserUIManager userConsoleManager, TaskUIManager taskConsoleManager, ProjectUIManager projectConsoleManager)
        {
            _userConsoleManager = userConsoleManager;
            _taskConsoleManager = taskConsoleManager;
            _projectConsoleManager = projectConsoleManager;
        }

        public async Task StartAsync()
        {
            var userDict = new Dictionary<int, Func<Task<Result<User>>>>()
            {
                {1, _userConsoleManager.Create },
                {2, _userConsoleManager.Authentificate }
            };
            await _taskConsoleManager.SendMessage();

            var loop = true;
            while (loop)
            {
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Authentifacate");
                int operationNumber = InputValidator.IntegerValidator();

                if(userDict.ContainsKey(operationNumber))
                {
                    loop = userDict[operationNumber].Invoke().Result.IsSuccessful;
                }
                else
                {
                    Console.WriteLine("Invalid operation number");
                    Console.ReadKey();
                    Console.Clear();
                    loop = true;
                }               
            }

            Console.Clear();

            if(!_userConsoleManager.IsAdminSessionNow)
            {
                await StandartSession();
            }
            else
            {
                await AdminSession();
            }
        }

        public async Task StandartSession()
        {
            while (true)
            {
                Console.WriteLine("1. Task oparations");
                Console.WriteLine("2. Project oparations");
                Console.WriteLine("3. User profile oparations");
                Console.WriteLine("4. Exit");

                var action = new Dictionary<int, Func<Task>>()
                {
                    {1, _taskConsoleManager.PerformOperations },
                    {2, _projectConsoleManager.PerformOperations },
                    {3, _userConsoleManager.PerformOperations }
                };

                int input = InputValidator.IntegerValidator();

                if (input == 4)
                {
                    break;
                }
                else if (action.ContainsKey(input))
                {
                    await action[input].Invoke();
                }
                else
                {
                    Console.WriteLine("Invalid oparation number");
                }

            }
        }

        public async Task AdminSession()
        {
            while (true)
            {
                Console.WriteLine("1. Task oparations");
                Console.WriteLine("2. Project oparations");
                Console.WriteLine("3. User profile oparations");
                Console.WriteLine("4. Exit");

                var action = new Dictionary<int, Func<Task>>()
                {
                    {1, _taskConsoleManager.PerformAdminOperations },
                    {2, _projectConsoleManager.PerformAdminOperations },
                    {3, _userConsoleManager.PerformAdminOperations }
                };

                int input = InputValidator.IntegerValidator();

                if (input == 4)
                {
                    break;
                }
                else if (action.ContainsKey(input))
                {
                    await action[input].Invoke();
                }
                else
                {
                    Console.WriteLine("Invalid oparation number");
                }

            }
        }
    }
}
