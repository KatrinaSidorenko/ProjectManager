using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserAuthenticationService _userAuthenticationService;
        public ProjectController(IProjectService projectService, 
            IUserAuthenticationService userAuthenticationService,
            UserManager<AppUser> userManager)
        {
            _projectService = projectService;
            _userAuthenticationService = userAuthenticationService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Project, ProjectDetailsViewModel>());
            var mapper = new Mapper(config);

            List<ProjectDetailsViewModel> projectsVM;
            if(isAdmin)
                projectsVM = mapper.Map<List<ProjectDetailsViewModel>>(await _projectService.GetAll());
            else
                projectsVM = mapper.Map<List<ProjectDetailsViewModel>>(await _projectService.GetUserProjects(user));
            
            return View(projectsVM);
        }

        [HttpGet]
        [Route("DetailsProject/{id}")]
        public async Task<IActionResult> DetailsProject(Guid id)
        {
            var project = await _projectService.GetById(id);

            if (project == null)
            {
                return NotFound();
            }

            var completedAssigments = await _projectService.ProgressCheck(id);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Project, ProjectViewModel>());
            var mapper = new Mapper(config);
            var projectVM = mapper.Map<ProjectViewModel>(project);

            var allProjectUsers = await _projectService.GetAllProjectUsers(id);
            projectVM.AllProjectUsers = allProjectUsers;
            projectVM.AmountOfCompletedAssigments = completedAssigments;

            return View(projectVM);
        }

        [Authorize(Roles = "StakeHolder")]
        public async Task<IActionResult> Create()
        {
            var projectVM = new ProjectCreateViewModel();
            var users = await _userAuthenticationService.GetUsersForTaskCompliting();
            var userCheckboxes = new List<AppUserCheckBoxViewModel>();
            foreach (var user in users)
            {
                var checkbox = new AppUserCheckBoxViewModel
                {
                    IsActive = false,
                    Description = $"{user.UserName} - {user.Email} ",
                    Value = user
                };

                userCheckboxes.Add(checkbox);
            }

            projectVM.ResponsibleUsersCheckboxes = userCheckboxes;

            return View(projectVM);
        }

        [HttpPost]
        [Authorize(Roles = "StakeHolder")]
        public async Task<IActionResult> CreateProject(ProjectCreateViewModel model)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectCreateViewModel, Project>());
            var mapper = new Mapper(config);
            var project = mapper.Map<Project>(model);
            var projectOwner = await _userAuthenticationService.GetUserById(model.ProjectOwnerId);
            project.ProjectOwner = projectOwner.Data;

            var result = await _projectService.AddProject(project);

            if(!result.IsSuccessful)
                TempData["Error"] = result.Message;

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "StakeHolder")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _projectService.GetById(id);

            if (project == null)
                return NotFound();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Project, ProjectDeleteViewModel>());
            var mapper = new Mapper(config);
            var projectVM = mapper.Map<ProjectDeleteViewModel>(project);

            return View(projectVM);
        }

        [Authorize(Roles = "StakeHolder")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _projectService.GetById(id);

            if (project == null)
                return NotFound();

            var result = await _projectService.DeleteProject(id);

            if (!result.IsSuccessful)
                TempData["Error"] = result.Message;

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "StakeHolder")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var project = await _projectService.GetById(id);

            if(project == null)
                return View("Error");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Project, ProjectEditViewModel>());
            var mapper = new Mapper(config);
            var projectEditVM = mapper.Map<ProjectEditViewModel>(project);

            var users = await _userAuthenticationService.GetUsersForTaskCompliting();
            var userCheckboxes = new List<AppUserCheckBoxViewModel>();
            foreach (var user in users)
            {
                var checkbox = new AppUserCheckBoxViewModel
                {
                    IsActive = user.UserProjects.Contains(project),
                    Description = $"{user.UserName} - {user.Email} ",
                    Value = user
                };

                userCheckboxes.Add(checkbox);
            }

            projectEditVM.ResponsibleUsersCheckboxes = userCheckboxes;

            return View(projectEditVM);
        }

        [HttpPost]
        [Authorize(Roles = "StakeHolder")]
        public async Task<IActionResult> EditProject(ProjectEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit assigment");
                return View("Edit", model);
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectEditViewModel, Project>());
            var mapper = new Mapper(config);
            var project = mapper.Map<Project>(model);
            project.CurrentProjectOwnerId = model.ProjectOwnerId;
            var user = await _userAuthenticationService.GetUserById(project.CurrentProjectOwnerId);
            project.ProjectOwner = user.Data;

            var result = await _projectService.UpdateProject(project);

            if(!result.IsSuccessful)
                TempData["Error"] = result.Message;

            return RedirectToAction("Index", "Home");
        }
    }
}
