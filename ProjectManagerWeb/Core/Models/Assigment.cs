using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enums;

namespace Core.Models
{
    public class Assigment : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public AssigmentStatus Status { get; set; }
        public Guid? CurrentProjectId { get; set; } 
        public virtual Project? AssigmentProject { get; set; } 
        public bool EmailIsSent { get; set; }
        public virtual IList<AssigmentFile> AssigmentFiles { get; set; } = new List<AssigmentFile>();
        public virtual IList<UserAssigment> UserAssigments { get; set; } = new List<UserAssigment>();

    }
}