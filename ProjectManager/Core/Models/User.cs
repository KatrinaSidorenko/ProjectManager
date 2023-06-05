using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public IList<Guid> Tasks = new List<Guid>();
        public UserStatus UserStatus { get; set; }
        
        public User(string userName, string email , string password, UserStatus status)
        {
            Name = userName;
            Password = password;
            Email = email;
            UserStatus = status;
        }
    }
}
