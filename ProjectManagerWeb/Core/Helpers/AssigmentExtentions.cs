using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Helpers
{
    public static class AssigmentExtentions
    {
        public static List<Assigment> SortAssigmentsByPriority(this List<Assigment> assigments) 
        { 
            if(assigments != null)
            {
                assigments.OrderByDescending(x => x.Priority).ToList();
            }

            return assigments;
        }
    }
}
