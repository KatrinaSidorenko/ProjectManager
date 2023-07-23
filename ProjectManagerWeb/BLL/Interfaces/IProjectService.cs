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
        Task<Result<bool>> AddProject(Project project);
        Task<Result<bool>> DeleteProject(Guid id);
        Task<double> ProgressCheck(Guid projectId);
        Task<IList<Project>> GetUserProjects(AppUser user);
        Task<List<AppUser>> GetAllProjectUsers(Guid projectId);
        Task<Result<bool>> UpdateProject(Project project);
    }
}
