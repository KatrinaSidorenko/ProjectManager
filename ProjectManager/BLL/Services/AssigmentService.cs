using BLL.Interfaces;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Models;
using Core.Enums;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections;

namespace BLL.Services
{
    public class AssigmentService : GenericService<Assigment>, IAssigmentService
    {
        private IUserService _userService;
        public AssigmentService(IRepository<Assigment> repository, IUserService userService) : base(repository)
        {
            _properties = typeof(Assigment).GetProperties();
            _userService = userService;
        }

        public async Task<IList<Assigment>> GetUserTasks(IList<Guid> tasksId)
        {
            var tasks = new List<Assigment>();

            foreach (var taskId in tasksId)
            {
                tasks.Add(await GetById(taskId));
            }

            return tasks;
        }

        public async Task ChangeTaskDescription(Guid id, string TaskDescription)
        {
            await GenericEdit(id, TaskDescription, "Description");
        }

        public async Task ChangeTaskExecutor(Guid id, User newExecutor)
        {
            await GenericEdit(id, newExecutor, "Executor");
        }

        public async Task ChangeTaskName(Guid id, string newName)
        {
            await GenericEdit(id, newName, "Name");
        }

        public async Task ChangeTaskPriority(Guid id, int newPriority)
        {
            await GenericEdit(id, newPriority, "Priority");
        }

        public async Task ChangeTaskStartDate(Guid id, DateTime newStartTime)
        {
            await GenericEdit(id, newStartTime, "StartDate");
        }

        public async Task ChangeTaskStatus(Guid id, AssigmentStatus newStatus)
        {
            var task = await GetById(id);
            foreach(var user in task.Executors)
            {
                string emailBody = $"Dear {user.Name}, the task '{task.Name.ToUpper()}' was changed." +
                    $"Task status changes: from {task.Status} to {newStatus}";
                EmailSender.SendMessage(user, "The status of the task has been changed", emailBody);
            }
            await GenericEdit(id, newStatus, "Status");
        }

        public async Task CreateTask(Assigment task)
        {
            if(task != null)
            {
                task.Id = Guid.NewGuid();

                foreach (var user in task.Executors)
                {
                    user.Tasks.Add(task.Id);
                    await _userService.Update(user.Id, user);
                }

                await Add(task);
            }
        }

        public async Task DeleteTask(Guid id)
        {
            if(id != Guid.Empty)
            {
                var task = await GetById(id);

                foreach (var user in task.Executors)
                {
                    var index = user.Tasks.IndexOf(id);
                    user.Tasks.RemoveAt(index);
                    await _userService.Update(user.Id, user);
                }
                await Delete(id);
            }
        }

        public async Task ChangeTaskEndDate(Guid id, DateTime newEndTime)
        {
            await GenericEdit(id, newEndTime, "EndDate");

        }

        public async Task AddExecutorsToTask(Guid taskId, IList<User> users)
        {
            var task = await GetById(taskId);
            var taskExecutorsId = task.Executors.Select(executor  => executor.Id).ToList();
            foreach (var user in users)
            {
                if (!taskExecutorsId.Contains(user.Id))
                {
                    task.Executors.Add(user);
                    user.Tasks.Add(task.Id);
                }
                await _userService.Update(user.Id, user);
            }

            await Update(taskId, task);
        }

        public async Task AddTaskToProject(Guid taskId, Project project)
        {
            var task = await GetById(taskId);
            if(task.ProjectId != project.Id)
            {
                task.ProjectId = project.Id;
                await Update(taskId, task);
            }            
        }

        public async Task RemoveTaskFromProject(Guid taskId, Project project)
        {
            var task = await GetById(taskId);

            if(task.ProjectId == project.Id)
            {
                task.ProjectId = Guid.Empty;
                await Update(taskId, task);
            }          
        }

        public async Task UpdateData()
        {
            var users = await _userService.GetAll();

            foreach (var user in users)
            {
                if(user.Tasks.Count > 0)
                {
                    foreach (var taskId in user.Tasks)
                    {
                        var task = await GetById(taskId);
                        var executorsId = task.Executors.Select(ex => ex.Id).ToList();
                        var index = executorsId.IndexOf(user.Id);
                        if (index != -1)
                        {
                            task.Executors[index] = user;
                            await Update(taskId, task);
                        }
                        
                    }
                }               
            }
        }

        public async Task UploadFileToTask(Guid taskId, string filePath)
        {           
             if(filePath != string.Empty)
             {
                var task = await GetById(taskId);
                string path = Path.Combine($@"~\files\{task.Name}\");
                if(task.Files == null)
                {
                    task.Files = new List<string>();
                }
                
                if (!Directory.Exists(path))
                {
                  Directory.CreateDirectory(path);
                }

                var fileName = Path.GetFileName(filePath);
                path += fileName;
                task.Files.Add(path);
                await Update(taskId, task);
                File.Copy(filePath, path);
             }                          
        }

        public async Task SendEmailAboutOverdueTasks()
        {
            var tasks = await GetAll();
            foreach (var task in tasks)
            {
                if(task.EndDate < DateTime.Now)
                {
                    foreach(var user in task.Executors)
                    {
                        if (!task.EmailIsSent && task.Status != AssigmentStatus.Done)
                        {
                            EmailSender.SendMessage(user, "Task was overdue", $"TASK : {task.Name} time was overdue {task.EndDate}");
                            task.EmailIsSent = true;
                            await Update(task.Id, task);
                       }                       
                    }
                }
            }
        }

        public async Task<List<AssigmentStatus>> GetAvailableTaskStatuses(Assigment assigment)
        {
            var user = _userService.GetCurrentUser().Result;
            List<AssigmentStatus> list = null;
            if (user.Tasks.Contains(assigment.Id))
            {
                switch (user.UserStatus)
                {
                    case UserStatus.StakeHolder:
                        list = Enum.GetValues(typeof(AssigmentStatus)).Cast<AssigmentStatus>().ToList();
                        break;
                    case UserStatus.QA:
                        list = new List<AssigmentStatus>() { AssigmentStatus.InTesting, AssigmentStatus.InProgress, AssigmentStatus.NotStarted };
                        break;
                    case UserStatus.Developer:
                        list = new List<AssigmentStatus>() { AssigmentStatus.InProgress, AssigmentStatus.NotStarted };
                        break;
                    case UserStatus.Designer:
                        list = new List<AssigmentStatus>() { AssigmentStatus.NotStarted, AssigmentStatus.InProgress };
                        break;
                    default:
                        list = new List<AssigmentStatus>();
                        break;
                }
            }
            else
            {
                list = Enum.GetValues(typeof(AssigmentStatus)).Cast<AssigmentStatus>().ToList();
            }
            
            return list;
        }

        public async Task<bool> GetAddFilePermition(Guid taskId)
        {
            var user = await _userService.GetCurrentUser();

            return user.Tasks.Contains(taskId);
        }

        public async Task<List<Assigment>> GetAllProjectTasks(Guid projectId)
        {
            var tasks = await GetAll();
            var projectTaks = new List<Assigment>();

            foreach (var task in tasks)
            {
                if(task.ProjectId == projectId)
                {
                    projectTaks.Add(task);
                }
            }

            return projectTaks;
        }
    }
}
