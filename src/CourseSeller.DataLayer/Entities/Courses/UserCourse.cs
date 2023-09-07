using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.DataLayer.Entities.Courses
{
    public class UserCourse
    {
        [Key]
        public int UC_Id { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }


        public User User { get; set; }
        public Course Course { get; set; }
    }
}
