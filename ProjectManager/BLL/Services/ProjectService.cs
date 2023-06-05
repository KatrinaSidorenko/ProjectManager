using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.Interfaces;
using Core.Models;
using Core.Enums;
using DAL.Interfaces;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProjectService : GenericService<Project>, IProjectService
    {
        private IAssigmentService _taskService;
        private IUserService _userService;
        public ProjectService(IRepository<Project> repository, IAssigmentService taskService, IUserService userService) : base(repository)
        {
            _properties = typeof(Project).GetProperties();
            _taskService = taskService;
            _userService = userService;
        }

        public async Task<IList<Project>> GetUserProjects(User user)
        {
            var projects = await GetAll();
            var userProjetcs = new List<Project>();

            foreach(var project in projects)
            {
                var projectUsers = await GetAllProjectUsers(project);
                foreach(var userProject in projectUsers)
                {
                    if (userProject.Id.Equals(user.Id))
                    {
                        userProjetcs.Add(project);
                    }
                }
            }

            return userProjetcs;
        }

        public async Task AddProject(Project project)
        {
            if(project != null)
            {
                project.Id = Guid.NewGuid();
                await Add(project);
            }
        }
        public async Task ChangeProjectDescription(Guid id, string description)
        {
            await GenericEdit(id, description, "Description");
        }

        public async Task ChangeProjectName(Guid id, string name)
        {
            await GenericEdit(id, name, "Name");
        }

        public async Task ChangeProjectOwner(Guid id, User newOwner)
        {
            await GenericEdit(id, newOwner, "ProjectOwner");
        }

        public async Task DeleteProject(Guid id)
        {
            if(id != Guid.Empty)
            {                
                await Delete(id);
            }
        }

        public async Task<IList<Assigment>> GetAllProjectTasks(Guid projectID)
        {
            await SortTasksByPriority(projectID);
            var tasks = _taskService.GetAllProjectTasks(projectID);

            return tasks.Result;
        }

        public async Task<double> ProgressCheck(Guid projectId)
        {
            var project = await GetById(projectId);
            var projectTasks = _taskService.GetAllProjectTasks(projectId).Result;
            int amountOfDoneTasks = projectTasks.Count(task => task.Status.Equals(AssigmentStatus.Done));

            if(amountOfDoneTasks > 0)
            {
                return (amountOfDoneTasks * 100) / projectTasks.Count;
            }

            return 0;
        }

        public async Task SortTasksByPriority(Guid projectId)
        {
            var project = await GetById(projectId);
            var tasks = await _taskService.GetAllProjectTasks(projectId);

            for(int i = 1; i < tasks.Count; i++)
            {
                for(int j = 0; j < tasks.Count - i; j++)
                {
                    if (tasks[j].Priority < tasks[j + 1].Priority)
                    {
                        Assigment temp = tasks[j];
                        tasks[j] = tasks[j + 1];
                        tasks[j + 1] = temp;
                    }
                }
            }

            await Update(projectId, project);
        }

        private async Task<List<User>> GetAllProjectUsers(Project project)
        {
            var projectUsers = new List<User>() { project.ProjectOwner};
            var tasks = _taskService.GetAllProjectTasks(project.Id);
            foreach (var task in tasks.Result)
            {
                foreach(var user in task.Executors)
                {
                    if (!projectUsers.Contains(user))
                    {
                        projectUsers.Add(user);
                    }
                }
            }

            return projectUsers;
        }

        public async Task UpdateUserData()
        {
            var projects = await GetAll();
            var users = await _userService.GetAll();
            foreach (var project in projects)
            {
                var user = users.FirstOrDefault(user => user.Id == project.ProjectOwner.Id);
                project.ProjectOwner = user;
                await Update(project.Id, project);
            }
        }
    }
}
