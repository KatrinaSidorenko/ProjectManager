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
using Core.ViewModels;

namespace BLL.Services
{
    public class AssigmentService : GenericService<Assigment>, IAssigmentService
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IEmailService _emailService;
        public AssigmentService(IUnitOfWork repository, 
            IUserAuthenticationService userAuthenticationService,
            IEmailService emailService) : base(repository)
        {
            _repository = repository.Assigments;
            _userAuthenticationService = userAuthenticationService;
            _emailService = emailService;
        }


        public async Task<Result<bool>> CreateAssigment(Assigment assigment, List<string> responsibleUsers)
        {
            if (assigment == null)
                return new Result<bool>(false, "Fail to create assigment");

            assigment.Id = Guid.NewGuid();
            var project = await _unitOfWork.Projects.GetById((Guid)assigment.CurrentProjectId);
            assigment.AssigmentProject = project.Data;

            var assigmentResult = await _repository.Add(assigment);

            if (!assigmentResult.IsSuccessful)
                return new Result<bool>(false, "Fail to create task");

            if (responsibleUsers != null)
            {
                var userAssigments = new List<UserAssigment>();

                foreach (var userId in responsibleUsers)
                {
                    var userResult = await _userAuthenticationService.GetUserById(userId);

                    if (userResult == null)
                        break;

                    var user = userResult.Data;
                    userAssigments.Add(new UserAssigment() { Assigment = assigment, User = user });                    
                }

                var userAssigmentResult = await _unitOfWork.UserAssigments.AddRange(userAssigments);

                if (!userAssigmentResult.IsSuccessful)
                    return new Result<bool>(false, "Fail to add user assigments");
            }

            var saveResult = _unitOfWork.Complete();

            if (!(saveResult >= 0))
                return new Result<bool>(false, "Fail to save changes while assigment creating");


            return new Result<bool>(true);
        }

        public async Task DeleteTask(Guid id)
        {
            if(id != Guid.Empty)
            {
                var task = await _unitOfWork.Assigments.GetById(id);
                var assigments = await _unitOfWork.UserAssigments.GetAll();
                var allAss = assigments.Data.Where(ass => ass.AssigmentId == id);

                foreach(var assigment in allAss)
                {
                    await _unitOfWork.UserAssigments.Remove(assigment);
                    _unitOfWork.Complete();
                }

                await Delete(id);
            }
        }

        public async Task<List<Assigment>> GetAllProjectTasks(Guid projectId)
        {
            var tasks = await _repository.GetAll();
            var projectTaks = tasks.Data.Where(task => task.CurrentProjectId ==  projectId).ToList();
            _unitOfWork.Complete();

            return projectTaks;
        }

        public async Task<List<AppUser>> GetAllResponisbleForTaskUsers(Guid taskId)
        {
            var assigment = await _repository.GetById(taskId);

            if(assigment == null )
                return new List<AppUser>();

            var responsibleUsers = assigment.Data.UserAssigments.Select(assigment => assigment.User);

            return responsibleUsers.ToList();
        }

        public async Task<Result<bool>> UpdateAssigmentInfo(Assigment assigment, List<string> responsibleUsers)
        {
            if(assigment == null)
                return new Result<bool>(false, "Fail to update assigment");

            var tempAssigment = await _repository.GetByIdAsNoTrackingAssigemnt(assigment.Id);
            var currResponsibleUsers = tempAssigment.Data.UserAssigments.Select(u => u.UserId).ToList();           

            if (responsibleUsers != null)
            {
                var userAssigments = new List<UserAssigment>();

                foreach (var userId in responsibleUsers)
                {
                    var userResult = await _userAuthenticationService.GetUserById(userId);

                    if (userResult == null)
                        break;

                    var userAssigment = new UserAssigment() { Assigment = assigment, User = userResult.Data };

                    if (!currResponsibleUsers.Contains(userId))
                        userAssigments.Add(userAssigment);
                }

                if(userAssigments.Count > 0)
                    await _unitOfWork.UserAssigments.AddRange(userAssigments);                
            }
         
            var result = await _repository.Update(assigment);

            if (!result.IsSuccessful)
                return new Result<bool>(false, "Fail to update assigment");

            _unitOfWork.Complete();

            //Send email about tasks changes to all responsible users
            var emaiSentResult = await SendEmailAboutAssigmentnfoChanges(responsibleUsers, assigment);

            if(!emaiSentResult.IsSuccessful)
                return new Result<bool>(true, emaiSentResult.Message);

            return new Result<bool>(true, "Assigment was edited successfully");
        }

        private StringBuilder CreateEmailBody(Assigment newAssigment)
        {
            
            var body = new StringBuilder();

            body.AppendLine($"<h3> New assigment data: </h3> " 
            + $"<p> Name: {newAssigment.Name}</p>" 
            + $"<p> Description: {newAssigment.Description}</p>" 
            + $"<p> Start Date: {newAssigment.StartDate}</p>" 
            + $"<p> End Date: {newAssigment.EndDate}</p>" 
            + $"<p> Status: {newAssigment.Status}</p>");

            return body;
        }

        private async Task<Result<bool>> SendEmailAboutAssigmentnfoChanges(List<string> responsibleUsers, Assigment newAssigment)
        {
            try
            {
                var emailBody = CreateEmailBody(newAssigment).ToString();

                foreach (var userId in responsibleUsers)
                {
                    var userResult = await _userAuthenticationService.GetUserById(userId);

                    if (userResult == null)
                        break;

                    var user = userResult.Data;
                    var emailSubject = $"Dear {user.UserName}, the assigment ({newAssigment.Name}) data was changed";

                    if (await _userAuthenticationService.IsEmailConfirmed(user))
                        await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                }

                return new Result<bool>(true, "Email was sent succsessfully");
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, "Fail to sent email");
            }           
        }
    }
}
