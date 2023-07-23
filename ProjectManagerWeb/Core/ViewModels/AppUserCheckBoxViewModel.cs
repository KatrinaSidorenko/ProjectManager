using Core.Models;

namespace Core.ViewModels
{
    public class AppUserCheckBoxViewModel
    {
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public AppUser Value { get; set; }
    }
}
