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
using static Azure.Core.HttpHeader;

namespace BLL.Services
{
    public class ProjectService : GenericService<Project>, IProjectService
    {
        private IAssigmentService _taskService;
        public ProjectService(IUnitOfWork repository, IAssigmentService taskService) : base(repository)
        {
            _taskService = taskService;
            _repository = repository.Projects;
        }

        public async Task<IList<Project>> GetUserProjects(AppUser user)
        {
            var userProjects = user.UserProjects;
            var assigments = user.UserAssigments.ToList();

            foreach ( var assigment in assigments)
            {
                var project = assigment.Assigment.AssigmentProject;
                if (!userProjects.Contains(project))
                {
                    userProjects.Add(project);
                }
                
            }


            return userProjects;
        }

        public async Task<Result<bool>> AddProject(Project project)
        {
            if (project == null)
                return new Result<bool>(false, "Fail to create project");

            project.Id = Guid.NewGuid();
            await Add(project);

            var saveResult = _unitOfWork.Complete();

            if (!(saveResult >= 0))
                return new Result<bool>(false, "Fail to create project");

            return new Result<bool>(true);
        }
        public async Task<Result<bool>> DeleteProject(Guid id)
        {
            if(id != Guid.Empty)
            {
                var project = await GetById(id);
                if (project == null)
                    return new Result<bool>(false, "Fail to delete project");

                await _repository.Remove(project);
                _unitOfWork.Complete();

                return new Result<bool>(true);
            }

            return new Result<bool>(false, "Fail to delete project");
        }

        public async Task<double> ProgressCheck(Guid projectId)
        {
            var project = await GetById(projectId);
            if(project != null)
            {
                var projectTasks = project.Assigments;
                int amountOfDoneTasks = projectTasks.Count(task => task.Status.Equals(AssigmentStatus.Done));

                if (amountOfDoneTasks > 0)
                {
                    return (amountOfDoneTasks * 100) / projectTasks.Count;
                }
            }
            
            return 0;
        }

        public async Task<List<AppUser>> GetAllProjectUsers(Guid projectId)
        {
            var project = await GetById(projectId);
            var allProjectUsers = new List<AppUser>();

            foreach(var assigment in project.Assigments)
            {
                var users = assigment.UserAssigments.Select(u => u.User);

                foreach(var user in users)
                {
                    if(!allProjectUsers.Contains(user))
                        allProjectUsers.Add(user);
                }
            }

            return allProjectUsers;
        }

        public async Task<Result<bool>> UpdateProject(Project project)
        {
            if (project == null)
                return new Result<bool>(false, "Fail to update project");

            var projectTasks = await _taskService.GetAllProjectTasks(project.Id);
            project.Assigments = projectTasks;

            await Update(project.Id, project);
            _unitOfWork.Complete();

            return new Result<bool>(true);

        }
    }
}
