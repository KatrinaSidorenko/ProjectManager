using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CurrentProjectOwnerId { get; set; }
        public virtual AppUser? ProjectOwner { get; set; }
        public double? AmountOfCompletedAssigments { get; set; }
    }
}
