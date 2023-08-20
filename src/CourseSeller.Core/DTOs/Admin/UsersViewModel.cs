using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.Core.DTOs.Admin
{
    public class UsersViewModel
    {
        public List<User> Users { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
    }
}
