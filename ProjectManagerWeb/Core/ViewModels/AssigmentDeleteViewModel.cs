using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class AssigmentDeleteViewModel
    {
        public Guid AssigmentId { get; set; }
        public string AssigmentName { get; set; }
        public string ProjectName { get; set; }
    }
}
