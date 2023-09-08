using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.DataLayer.Entities.Courses
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }


        [MaxLength(700)]
        public string Content { get; set; }

        public DateTime CreateDateTime { get; set; }

        public bool IsDelete { get; set; }

        public bool IsAdminRead { get; set; }



        public Course Course { get; set; }
        public User User { get; set; }
    }
}
