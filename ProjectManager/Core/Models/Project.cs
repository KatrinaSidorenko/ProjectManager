using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public User ProjectOwner { get; set; }

        public Project(string name, string description, User projectOwner)
        {
            Name = name;
            Description = description;
            ProjectOwner = projectOwner;
        }
    }
}
