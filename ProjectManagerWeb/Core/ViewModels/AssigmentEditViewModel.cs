using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class AssigmentEditViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Task name is required")]
        public string? Name { get; set; }

        public string? Description { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        [Range(0, 100)]
        public int Priority { get; set; }
        public AssigmentStatus Status { get; set; }
        public Guid? CurrentProjectId { get; set; }
        public virtual Project? AssigmentProject { get; set; }
        public List<AppUserCheckBoxViewModel> ResponsibleUsersCheckboxes { get; set; } = new List<AppUserCheckBoxViewModel>();
        public List<string> ResponsibleUsers { get; set; }
        public List<AssigmentStatus>? PossibleAssigmentStatuses { get; set; }
    }
}
