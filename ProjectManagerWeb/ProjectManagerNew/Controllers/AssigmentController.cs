using AutoMapper;
using BLL.Interfaces;
using Core.Models;
using Core.ViewModels;
using Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagerNew.Controllers
{
    [Authorize]
    public class AssigmentController : Controller
    {
        private readonly IAssigmentService _assigmentService;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly UserManager<AppUser> _userManager;
		public AssigmentController(IAssigmentService assigmentService,
            IUserAuthenticationService userAuthenticationService,
            UserManager<AppUser> userManager)
        {
            _assigmentService = assigmentService;
            _userAuthenticationService = userAuthenticationService;
            _userManager = userManager;
		}
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("Create/{id}")]
        public async Task<IActionResult> Create(Guid id)
        {
            var assigmentVM = new AssigmentCreateViewModel() { CurrentProjectId = id };

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

            assigmentVM.ResponsibleUsersCheckboxes = userCheckboxes;

            return View(assigmentVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssigment(AssigmentCreateViewModel model)
        {
            if (!ModelState.IsValid) 
                return View("Create", model);

            var config = new MapperConfiguration(cfg => cfg.CreateMap<AssigmentCreateViewModel, Assigment>());
            var mapper = new Mapper(config);
            var assigment = mapper.Map<Assigment>(model);

            var result = await _assigmentService.CreateAssigment(assigment, model.ResponsibleUsers);

            if(!result.IsSuccessful)
                TempData["Error"] = result.Message;

            return RedirectToAction("Details", "Project");
        }

        [HttpGet]
		[Route("ShowProjectTasks/{id}")]
		public async Task<IActionResult> ShowProjectTasks(Guid id)
        {
            var projectTasks = await _assigmentService.GetAllProjectTasks(id);
            var sortedAssigments = projectTasks.SortAssigmentsByPriority();

            return View(sortedAssigments);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var assigment = await _assigmentService.GetById(id);

            if (assigment == null)
                return View("Error");

            var assigmentVM = new AssigmentDeleteViewModel()
            {
                AssigmentId = id,
                AssigmentName = assigment.Name,
                ProjectName = assigment.AssigmentProject.Name
            };

            return View(assigmentVM);
        }

        [HttpGet]
        [Route("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var assigment = await _assigmentService.GetById(id);

            if(assigment == null)
                return View("Error");

            await _assigmentService.Delete(id);

            return RedirectToAction("Details", "Project");
        }

        [HttpGet]
        public async Task<IActionResult> AssigmentDetails(Guid id)
        {
            var assigment = await _assigmentService.GetById(id);

            if (assigment == null)
                return View("Error");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Assigment, AssigmentDetailsViewModel>());
            var mapper = new Mapper(config);
            var assigmentDetailsVM = mapper.Map<AssigmentDetailsViewModel>(assigment);

            assigmentDetailsVM.ResponsibleUsers = await _assigmentService.GetAllResponisbleForTaskUsers(id);

            return View(assigmentDetailsVM);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var assigment = await _assigmentService.GetById(id);

            if(assigment == null)
                return View("Error");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Assigment, AssigmentEditViewModel>());
            var mapper = new Mapper(config);
            var assigmentEditVM = mapper.Map<AssigmentEditViewModel>(assigment);

            var users = await _userAuthenticationService.GetUsersForTaskCompliting();
            var userCheckboxes = new List<AppUserCheckBoxViewModel>();
            foreach (var user in users)
            {
                var checkbox = new AppUserCheckBoxViewModel
                {
                    IsActive = user.UserAssigments.Where(u => u.AssigmentId == id).Any(),
                    Description = $"{user.UserName} - {user.Email} ",
                    Value = user
                };

                userCheckboxes.Add(checkbox);
            }

            assigmentEditVM.ResponsibleUsersCheckboxes = userCheckboxes;

            return View(assigmentEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditAssigment(AssigmentEditViewModel assigmentVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit assigment");
                return View("Edit", assigmentVM);
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<AssigmentEditViewModel, Assigment>());
            var mapper = new Mapper(config);
            var assigment = mapper.Map<Assigment>(assigmentVM);

            var result = await _assigmentService.UpdateAssigmentInfo(assigment, assigmentVM.ResponsibleUsers);

            if (!result.IsSuccessful)
                TempData["Error"] = result.Message;

            return RedirectToAction("Details", "Project");
        }

        public async Task<IActionResult> DetailsUserAssigments()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userAssigments = currentUser.UserAssigments.Select(u => u.Assigment).ToList();
            var sortedAssigments = userAssigments.SortAssigmentsByPriority();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Assigment, AssigmentDetailsViewModel>());
            var mapper = new Mapper(config);
            var assigmentDetailsVM = mapper.Map<List<AssigmentDetailsViewModel>>(sortedAssigments);

            return View(assigmentDetailsVM);
        }
    }
}
