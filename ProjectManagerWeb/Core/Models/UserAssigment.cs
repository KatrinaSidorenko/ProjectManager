using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class UserAssigment : BaseEntity
    {
        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; } 
        public Guid AssigmentId { get; set; }
        public virtual Assigment? Assigment { get; set; }
    }
}
