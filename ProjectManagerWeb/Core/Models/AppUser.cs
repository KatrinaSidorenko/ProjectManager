using Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AppUser : IdentityUser
    {
        //public string Name { get; set; }
        public virtual IList<Project> UserProjects { get; set; } = new List<Project>();
        public virtual IList<UserAssigment> UserAssigments { get; set; } = new List<UserAssigment>();
    }
}
