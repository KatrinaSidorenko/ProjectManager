using BLL.Interfaces;
using Core.Helpers;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UI.Interfaces;

namespace UI.ConsoleManagers
{
    public class ProjectUIManager : MainUIManager<Project, IProjectService>, IConsoleManager<Project>
    {
        private UserUIManager _userConsoleManager;
        private Dictionary<string, object> _projectPropertys = new Dictionary<string, object>();
        public ProjectUIManager(IProjectService service,  UserUIManager userConsoleManager) : base(service)
        {
            _userConsoleManager = userConsoleManager;
        }

        public async Task PerformOperations()
        {
            await Service.UpdateUserData();

            Dictionary<int, Func<Task>> oparations = new Dictionary<int, Func<Task>>()
            {
                {1, Create },
                {2, ShowAllProjects },
                {3, ShowUserProjects },
                {4, Delete },
                {5, Edit },
                {6, ShowSortedProjectTasks }
            };
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Create project");
                Console.WriteLine("2. See all projects");
                Console.WriteLine("3. See projects of concrete user");
                Console.WriteLine("4. Dlete project");
                Console.WriteLine("5. Edit project");
                Console.WriteLine("6. Sort project tasks");
                Console.WriteLine("7. Exit");

                int input = InputValidator.IntegerValidator();

                if (input == 7)
                {
                    break;
                }
                else if (oparations.ContainsKey(input))
                {
                    Console.Clear();
                    await oparations[input].Invoke();
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Invalid oparation number");
                }
            }

            Console.Clear();
        }
        public async Task ShowAllProjects()
        {
            var projects = await GetAll();
            await ShowObjects(projects.ToList(), typeof(Project));
        }

        private async Task ShowSortedProjectTasks()
        {
            Console.WriteLine("Avalable projects: ");
            var proj = await Service.GetAll();
            var projects = proj.ToArray();

            if (projects.Length != 0)
            {
                for (var i = 0; i < projects.Length; i++)
                {
                    Console.WriteLine($"--------------------------------- ({i + 1})");
                    await ShowObjects(new List<Project> { projects[i] }, typeof(Project));
                }
                int projectNumber = InputValidator.IndexValidator(projects.Length);

                var project = projects[projectNumber - 1];
                await Service.GetAllProjectTasks(project.Id);
            }
            else
            {
                Console.WriteLine("No projects");
                Console.ReadKey();
            }         
        }
        private async Task GetProjectName()
        {
            Console.WriteLine("Enter project name: ");
            string name = Console.ReadLine();

            _projectPropertys.Add("Name", name);
        }

        private async Task GetProjectDescription()
        {
            Console.WriteLine("Enter project description: ");
            string name = Console.ReadLine();

            _projectPropertys.Add("Description", name);
        }

        private async Task GetProjectOwner()
        {
            Console.WriteLine("Avalible users: ");
            await _userConsoleManager.ShowAllStakeHolders();

            Console.Write("Enter project owner name: ");
            string owner = Console.ReadLine();

            try
            {
                var projectOwner = await _userConsoleManager.GetByPredicate(user => user.Name == owner);
                _projectPropertys.Add("ProjectOwner", projectOwner);              
            }
            catch
            {
                Console.WriteLine("Failed to get project owner");
            }

            Console.ReadKey();
        }

        public async Task Create()
        {
            await GetProjectName();
            await GetProjectDescription();
            await GetProjectOwner();

            try
            {
                var newProject = await CreateObject(_projectPropertys.Values.ToArray());
                await Service.AddProject(newProject);
            }
            catch
            {
                Console.WriteLine("Failed to create project");
            }
            
            _projectPropertys.Clear();
        }

        public async Task ShowUserProjects()
        {
            Console.WriteLine("Enter your name for seeing all your projects: ");
            string name = Console.ReadLine();

            try
            {
                var user = await _userConsoleManager.GetByPredicate(user => user.Name == name);
                var tasks = await Service.GetUserProjects(user);
                await ShowObjects(tasks.ToList(), typeof(Project));
            }
            catch
            {
                Console.WriteLine("This name wasn't found");
                await ReinvokeMethodHelper(ShowUserProjects, PerformOperations);
            }
        }

        public async Task Delete()
        {
            Console.WriteLine("Avalable projects: ");
            var proj = await Service.GetAll();
            var projects = proj.ToArray();
            for (var i = 0; i < projects.Length; i++)
            {
                Console.WriteLine($"--------------------------------- ({i + 1})");
                await ShowObjects(new List<Project> { projects[i] }, typeof(Project));
            }

            if(projects.Length > 0)
            {
                int projectNumber = InputValidator.IntegerValidator();
                var project = projects[projectNumber - 1];

                await Service.DeleteProject(project.Id);
            }
            else
            {
                Console.WriteLine("No tasks");
            }
            
        }

        public async Task Edit()
        {
            Console.WriteLine("Avalable projects for editing: ");
            var proj = await Service.GetAll();
            var projects = proj.ToArray();
            if (projects.Length > 0)
            {
                for (var i = 0; i < projects.Length; i++)
                {
                    Console.WriteLine($"---------------------------------{i + 1}");
                    await ShowObjects(new List<Project> { projects[i] }, typeof(Project));
                }
                int projectNumber = InputValidator.IndexValidator(projects.Length);
                Project project = projects[projectNumber - 1];

                var projectOparations = new Dictionary<int, Func<Task>>()
                {
                    {1, GetProjectName },
                    {2, GetProjectDescription},
                    {3, GetProjectOwner},
                };

                Console.WriteLine("Availble editing oparations: ");
                Console.WriteLine("1. Change project name");
                Console.WriteLine("2. Change project description");
                Console.WriteLine("3. Change project owner");
                Console.WriteLine("4. Show project % of comleted tasks");

                int operationNumber = InputValidator.IntegerValidator();

                if (operationNumber == 4)
                {
                    await ShowProcentOfCompletedTasks(project);
                }
                else
                {
                    await projectOparations[operationNumber].Invoke();
                }

                await EditProjectHelper(project, operationNumber);

                _projectPropertys.Clear();
            }
            else
            {
                Console.WriteLine("No projects");
                Console.ReadKey();
            }
        }

        private async Task EditProjectHelper(Project project,int operationNum)
        {
            switch (operationNum)
            {
                case 1:
                    await Service.ChangeProjectName(project.Id, (string)_projectPropertys["Name"]);
                    break;
                case 2:
                    await Service.ChangeProjectDescription(project.Id, (string)_projectPropertys["Description"]);
                    break;
                case 3:
                    await Service.ChangeProjectOwner(project.Id, (User)_projectPropertys["ProjectOwner"]);
                    break;
                default:
                    break;
            }
        }

        private async Task ShowProcentOfCompletedTasks(Project project)
        {
            Console.Clear();
            Console.WriteLine($"% of comleted tasks PROJECT NAME --> {project.Name}: {await Service.ProgressCheck(project.Id)}");
            Console.ReadLine();
        }
    }
}
