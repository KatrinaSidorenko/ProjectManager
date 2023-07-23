using BLL.Interfaces;
using Core.Enums;
using Core.Helpers;
using Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Interfaces;

namespace UI.ConsoleManagers
{
    public class TaskUIManager : MainUIManager<Assigment, IAssigmentService>, IConsoleManager<Assigment>
    {
        private UserUIManager _userConsoleManager;
        private ProjectUIManager _projectConsoleManager;
        private Dictionary<string, object> _taskPropertyValues = new Dictionary<string, object>();
        public TaskUIManager(IAssigmentService service, UserUIManager userManager, ProjectUIManager projectManager) : base(service)
        {
            _userConsoleManager = userManager;
            _projectConsoleManager = projectManager;
        }

        public async Task PerformOperations()
        {
            await Service.UpdateData();
            Dictionary<int, Func<Task>> oparations = new Dictionary<int, Func<Task>>()
            {
                {1, Create },
                //{2, ShowAllTasks },
                {2, ShowUserTasks },
                {3, Delete },
                {4, Edit },
                {5, AddTaskToProject },
                {6, RemoveTaskFromProject },
                {7, SeeAllProjectTasks}
            };

            while(true)
            {
                Console.Clear();
                Console.WriteLine("1. Create task");
                //Console.WriteLine("2. See all tasks");
                Console.WriteLine("2. See your tasks");
                Console.WriteLine("3. Delete task");
                Console.WriteLine("4. Edit task");
                Console.WriteLine("5. Add task to project");
                Console.WriteLine("6. Remove task from project");
                Console.WriteLine("7. See all project tasks");
                Console.WriteLine("8. Exit");

                int input = InputValidator.IntegerValidator();

                if(input == 8)
                {
                    break;
                }
                else if(oparations.ContainsKey(input))
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

        public async Task PerformAdminOperations()
        {
            await Service.UpdateData();
            Dictionary<int, Func<Task>> oparations = new Dictionary<int, Func<Task>>()
            {
                {1, ShowAllTasks },
                {2, Delete },
                {3, SeeAllProjectTasks}
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. See all tasks");
                Console.WriteLine("2. Delete task");
                Console.WriteLine("3. See all project tasks");
                Console.WriteLine("4. Exit");

                int input = InputValidator.IntegerValidator();

                if (input == 4)
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

        public async Task Create()
        {
            await GetTaskName();
            await GetTaskDescription();
            await GetStratDate();
            await GetEndDate();
            await GetTaskExecutors();
            await GetTaskPriority();
            await GetTaskStatus();

            try
            {
                var newTask = await CreateObject(_taskPropertyValues.Values.ToArray());
                await Service.CreateTask(newTask);
            }
            catch
            {
                Console.WriteLine("Failed to create task");
            }  
            
            _taskPropertyValues.Clear();            
        }

        public async Task ShowAllTasks()
        {
            var tasks = await GetAll();
            await ShowObjects(tasks.ToList(), typeof(Assigment));
        }

        private async Task GetTaskName()
        {
            Console.WriteLine("Enter task name: ");
            string name = Console.ReadLine();
            _taskPropertyValues.Add("Name", name);
        }

        private async Task GetTaskDescription()
        {
            Console.WriteLine("Enter task description: ");
            string description = Console.ReadLine();
            _taskPropertyValues.Add("Description", description);

        }

        private async Task GetStratDate()
        {
            DateTime stratDate = InputValidator.DateTimeValidator("Enter task strat date: ");

            _taskPropertyValues.Add("StardDate", stratDate);

        }
        private async Task GetEndDate()
        {
            DateTime endDate = InputValidator.DateTimeValidator("Enter task end date: ");

            _taskPropertyValues.Add("EndDate", endDate);

        }

        private async Task GetTaskExecutors()
        {
            var taskExecutors = new List<User>();
            Console.WriteLine("Available users for task compliting: ");
            Console.WriteLine();
            await _userConsoleManager.ShowAllUsers();
            int loop = 0;

            while(loop != 1)
            {
                Console.Write("Enter user name: ");
                string executorName = Console.ReadLine();
                try
                {
                    var user = await _userConsoleManager.GetByPredicate(user => user.Name == executorName);
                    if (!taskExecutors.Select(user => user.Id).ToList().Contains(user.Id))
                    {
                        taskExecutors.Add(user);
                    }                                                       
                }
                catch
                {
                    Console.WriteLine("Invalid input user name");
                }

                Console.WriteLine("If you want stop adding users, enter 1");
                loop = InputValidator.IntegerValidator();
            }

            _taskPropertyValues.Add("Executors", taskExecutors);
        }

        private async Task GetTaskPriority()
        {
            var priority = await GetTaskPriorityHelper();

            _taskPropertyValues.Add("Priority", priority);
 
        }

        private async Task GetTaskStatus()
        {
            await BaseEditTaskStatus(Enum.GetValues(typeof(AssigmentStatus)).Cast<AssigmentStatus>().ToList());     
        }

        private async Task EditTaskStatus()
        {
            var availableUserStats = await Service.GetAvailableTaskStatuses((Assigment)_taskPropertyValues["Task"]);
            await BaseEditTaskStatus(availableUserStats);
        }

        private async Task BaseEditTaskStatus(List<AssigmentStatus> stusesArray)
        {
            Console.WriteLine();
            Console.WriteLine("Available task statuses: ");

            foreach (var taskStatus in stusesArray)
            {
                Console.WriteLine($"\t{taskStatus}");
            }

            Console.Write("Enter status: ");

            AssigmentStatus result;
            string status = Console.ReadLine();

            if (Enum.TryParse(status, out result))
            {
                _taskPropertyValues.Add("Status", result);
            }
            else
            {
                Console.WriteLine("Invalid input status");
                await ReinvokeMethodHelper(GetTaskStatus, PerformOperations);
            }
        }

        public async Task ShowUserTasks()
        {
            //Console.WriteLine("Enter your name for seeing all your tasks: ");
            //string name = Console.ReadLine();

            try
            {
                //var user = await _userConsoleManager.GetByPredicate(user => user.Name == name);
                var tasks = await Service.GetUserTasks();
                if(tasks.Count > 0)
                {
                    await ShowObjects(tasks.ToList(), typeof(Assigment));
                }
                else
                {
                    Console.WriteLine("No tasks for you");
                }
            }
            catch
            {
                //Console.WriteLine("This name wasn't found");
                Console.WriteLine("Can't get user tasks");
                await ReinvokeMethodHelper(ShowUserTasks, PerformOperations);
            }           
        }

        public async Task Delete()
        {
            Console.WriteLine("Available tasks: ");
            IList<Assigment> alltasks;
            if(!_userConsoleManager.IsAdminSessionNow)
            {
                alltasks = await Service.GetUserTasks();
            }
            else
            {
                alltasks = await Service.GetAll();
            }
            
            var tasks = alltasks.ToArray();

            if(tasks.Length != 0)
            {
                for (var i = 0; i < tasks.Length; i++)
                {
                    Console.WriteLine($"--------------------------- ({i + 1})");
                    await ShowObjects(new List<Assigment> { tasks[i] }, typeof(Assigment));
                }

                int taskIndex = InputValidator.IndexValidator(tasks.Length);
                var task = tasks[taskIndex - 1];
                await Service.DeleteTask(task.Id);
            }
            else
            {
                Console.WriteLine("No tasks for deleting");
                Console.ReadKey();
            }                     
        }

        public async Task Edit()
        {
            Console.WriteLine("Available tasks name: ");
            var alltasks = await Service.GetUserTasks();
            var tasks = alltasks.ToArray();

            if (tasks.Length != 0)
            {
                for (var i = 0; i < tasks.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {tasks[i].Name}");
                }
                int taskNumber = InputValidator.IndexValidator(tasks.Length);

                if(taskNumber <= tasks.Length && taskNumber > 0)
                {
                    var task = tasks[taskNumber - 1];

                    _taskPropertyValues.Add("Task", task);

                    Console.WriteLine();
                    Console.WriteLine("Current task data: ");
                    await ShowObjects(new List<Assigment> { task }, typeof(Assigment));

                    var editDictionary = new Dictionary<int, Func<Task>>()
                {
                    {1, GetTaskName},
                    {2, GetTaskDescription},
                    {3, GetTaskPriority},
                    {4, EditTaskStatus },
                    {5, GetStratDate },
                    {6, GetEndDate },
                    {7, GetTaskExecutors },
                    {8, GetFilePath }
                };

                    Console.WriteLine();
                    Console.WriteLine("Edit oparations: ");
                    Console.WriteLine("1. Edit task name");
                    Console.WriteLine("2. Edit task description");
                    Console.WriteLine("3. Edit task priority");
                    Console.WriteLine("4. Edit task status");
                    Console.WriteLine("5. Edit task start date");
                    Console.WriteLine("6. Edit task end date");
                    Console.WriteLine("7. Add user to task");
                    Console.WriteLine("8. Add file to task");

                    int operationNumber = InputValidator.IntegerValidator();
                    await editDictionary[operationNumber].Invoke();
                    await EditTaskHelper(task, operationNumber);
                    _taskPropertyValues.Clear();
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
            else
            {
                Console.WriteLine("No tasks for editing");
            }
            
        }

        private async Task SeeAllProjectTasks()
        {
            Console.WriteLine("Available projects: ");
            IEnumerable<Project> proj;
            
            if(!_userConsoleManager.IsAdminSessionNow)
            {
                proj = await _projectConsoleManager.GetUserProjects();
            }
            else
            {
                proj = await _projectConsoleManager.GetAll();
            }
            var projects = proj.ToArray();

            if (projects.Length > 0)
            {
                for (var i = 0; i < projects.Length; i++)
                {
                    Console.WriteLine($"--------------------------- ({i + 1})");
                    await ShowObjects(new List<Project> { projects[i] }, typeof(Project));
                }

                int projectNumber = InputValidator.IndexValidator(projects.Length);
                var project = projects[projectNumber - 1];

                Console.WriteLine();
                Console.WriteLine("-------------------------------");
                Console.WriteLine($"TASKS OF '{project.Name}'");
                var tasksOfProject = await Service.GetAllProjectTasks(project.Id);
                await ShowObjects(tasksOfProject.ToList(), typeof(Assigment));
            }
            else
            {
                Console.WriteLine("No projects");

            }

            
        }

        private async Task EditTaskHelper(Assigment task, int operationNumber)
        {
            switch (operationNumber)
            {
                case 1:
                    await Service.ChangeTaskName(task.Id, (string)_taskPropertyValues["Name"]);
                    break;
                case 2:
                    await Service.ChangeTaskDescription(task.Id, (string)_taskPropertyValues["Description"]);
                    break;
                case 3:
                    await Service.ChangeTaskPriority(task.Id, (int)_taskPropertyValues["Priority"]);
                    break;
                case 4:
                    await Service.ChangeTaskStatus(task.Id, (AssigmentStatus)_taskPropertyValues["Status"]);
                    break;
                case 5:
                    await Service.ChangeTaskStartDate(task.Id, (DateTime)_taskPropertyValues["StartDate"]);
                    break;
                case 6:
                    await Service.ChangeTaskEndDate(task.Id, (DateTime)(_taskPropertyValues["EndDate"]));
                    break;
                case 7:
                    await Service.AddExecutorsToTask(task.Id, (IList<User>)(_taskPropertyValues["Executors"]));
                    break;
                case 8 :
                    await Service.UploadFileToTask(task.Id, (string)(_taskPropertyValues["Path"]));
                    break;
                default:
                    break;
            }
        }

        private async Task<int> GetTaskPriorityHelper()
        {
            Console.Write("Enter task priority (from 0 to 50): ");
            int priority;
            if (int.TryParse(Console.ReadLine(), out priority) && priority >= 0 && priority <= 50)
            {
                return priority;
            }
            else
            {
                Console.WriteLine("Invalid input task priority");
                return await GetTaskPriorityHelper();
            }
        }

        private async Task AddTaskToProject()
        {
            var projectAndTask = await BaseGetProjectAndTask();

            if(projectAndTask.Item1 != null)
            {
                await Service.AddTaskToProject(projectAndTask.Item2.Id, projectAndTask.Item1);
            }
        }

        private async Task RemoveTaskFromProject()
        {
            var projectAndTask = await BaseGetProjectAndTask();

            if(projectAndTask.Item1 != null )
            {
                await Service.RemoveTaskFromProject(projectAndTask.Item2.Id, projectAndTask.Item1);
            }         
        }

        private async Task<(Project, Assigment)> BaseGetProjectAndTask()
        {
            Console.WriteLine("Available projects: ");
            var proj = await _projectConsoleManager.GetUserProjects();
            var projects = proj.ToArray();

            if(projects.Length > 0)
            {
                for (var i = 0; i < projects.Length; i++)
                {
                    Console.WriteLine($"--------------------------- ({i + 1})");
                    await ShowObjects(new List<Project> { projects[i] }, typeof(Project));
                }

                int projectNumber = InputValidator.IndexValidator(projects.Length);
                var project = projects[projectNumber - 1];

                Console.WriteLine();
                Console.WriteLine("-------------------------------");

                Console.WriteLine("Available tasks: ");
                var alltasks = await Service.GetAll();
                var tasks = alltasks.ToArray();

                if(tasks.Length  > 0)
                {

                    for (var i = 0; i < tasks.Length; i++)
                    {
                        Console.WriteLine($"--------------------------- ({i + 1})");
                        await ShowObjects(new List<Assigment> { tasks[i] }, typeof(Assigment));
                    }

                    int taskNumber = InputValidator.IndexValidator(tasks.Length);
                    var task = tasks[taskNumber - 1];

                    return (project, task);
                }
                else
                {
                    Console.WriteLine("No available tasks");
                    return (null, null);
                }
            }
            else
            {
                Console.WriteLine("No projects");
                return (null, null);
            }

            
        }

        private async Task GetFilePath()
        {
            var task = (Assigment)_taskPropertyValues["Task"];
            var permition = Service.GetAddFilePermition(task.Id);
            if (permition.Result)
            {
                Console.WriteLine();
                Console.Write("Enter file path: ");
                string path = Console.ReadLine();

                if (path != string.Empty)
                {
                    _taskPropertyValues["Path"] = path;
                }
                
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("You can't add file to this task");
                _taskPropertyValues["Path"] = "";
                Console.ReadKey();
            }
            
        }

        public async Task SendMessage()
        {
            await Service.SendEmailAboutOverdueTasks();
        }


    }
}
