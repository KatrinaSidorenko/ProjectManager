using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProjectService : IGenericService<Project>
    {
        Task AddProject(Project project);
        Task DeleteProject(Guid id);
        Task ChangeProjectName(Guid id, string name);
        Task ChangeProjectDescription(Guid id, string description);
        Task ChangeProjectOwner(Guid id, User newOwner);
        Task SortTasksByPriority(Guid projectId);
        Task<double> ProgressCheck(Guid projectId);
        Task<IList<Assigment>> GetAllProjectTasks(Guid projectId);
        Task<IList<Project>> GetUserProjects();
        Task UpdateUserData();
    }
}
