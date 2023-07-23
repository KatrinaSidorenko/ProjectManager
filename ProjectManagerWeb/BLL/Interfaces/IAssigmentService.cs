using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Models;
using Core.ViewModels;

namespace BLL.Interfaces
{
    public interface IAssigmentService : IGenericService<Assigment>
    {
        Task<Result<bool>> CreateAssigment(Assigment assigment,List<string> responsibleUsers);
        Task DeleteTask(Guid id);
        Task<List<Assigment>> GetAllProjectTasks(Guid projectId);
        Task<List<AppUser>> GetAllResponisbleForTaskUsers(Guid taskId);
        Task<Result<bool>> UpdateAssigmentInfo(Assigment assigment, List<string> responsibleUsers);
    }
}
