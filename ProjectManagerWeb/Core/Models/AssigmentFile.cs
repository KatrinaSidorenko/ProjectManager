using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AssigmentFile : BaseEntity
    {
        public string? Adress { get; set; }
        public Guid? CurrentAssigmentId { get; set; }
        public virtual Assigment? Assigment { get; set; }
    }
}
