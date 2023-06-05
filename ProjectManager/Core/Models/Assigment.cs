using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enums;

namespace Core.Models
{
    public class Assigment : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public IList<User> Executors { get; set; }
        public AssigmentStatus Status { get; set; }
        public Guid ProjectId { get; set; }
        public bool EmailIsSent { get; set; }
        public IList<string> Files { get; set; }

        public Assigment(string name, string description, DateTime startDate, DateTime endDate, IList<User> executors, int priority, AssigmentStatus status)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Executors = executors;
            Priority = priority;          
            Status = status;
        }
    }
}
