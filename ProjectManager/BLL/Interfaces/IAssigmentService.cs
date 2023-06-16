using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Models;

namespace BLL.Interfaces
{
    public interface IAssigmentService : IGenericService<Assigment>
    {
        Task CreateTask(Assigment task);
        Task DeleteTask(Guid id);
        Task ChangeTaskName(Guid id, string newName);
        Task ChangeTaskDescription(Guid id, string description);
        Task ChangeTaskStartDate(Guid id, DateTime newStartTime);
        Task ChangeTaskEndDate(Guid id, DateTime newEndTime);
        Task ChangeTaskPriority(Guid id, int newPriority);
        Task ChangeTaskExecutor(Guid id, User newExecutor);
        Task ChangeTaskStatus(Guid id, AssigmentStatus newStatus);
        Task<IList<Assigment>> GetUserTasks();
        Task AddExecutorsToTask(Guid taskId, IList<User> users);
        Task AddTaskToProject(Guid taskId, Project project);
        Task RemoveTaskFromProject(Guid taskId, Project project);
        Task UpdateData();
        Task UploadFileToTask(Guid taskId, string filePath);
        Task SendEmailAboutOverdueTasks();
        Task<List<AssigmentStatus>> GetAvailableTaskStatuses(Assigment assigment);
        Task<bool> GetAddFilePermition(Guid taskId);
        Task<List<Assigment>> GetAllProjectTasks(Guid projectId);
    }
}
