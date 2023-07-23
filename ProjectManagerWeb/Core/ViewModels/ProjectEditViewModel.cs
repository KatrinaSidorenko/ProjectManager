using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ProjectEditViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Project name is required")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<AppUserCheckBoxViewModel> ResponsibleUsersCheckboxes { get; set; } = new List<AppUserCheckBoxViewModel>();
        public string ProjectOwnerId { get; set; }
        public virtual IList<Assigment> Assigments { get; set; } = new List<Assigment>();
    }
}
