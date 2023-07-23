using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class AssigmentDetailsViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public AssigmentStatus Status { get; set; }
        public List<AppUser> ResponsibleUsers { get; set; }
        public virtual Project? AssigmentProject { get; set; }
        public virtual IList<AssigmentFile> AssigmentFiles { get; set; } = new List<AssigmentFile>();
    }
}
