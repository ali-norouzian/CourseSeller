using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.DataLayer.Entities.Courses
{
    public class CourseVote
    {
        [Key]
        public int VoteId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string UserId { get; set; }


        public bool vote { get; set; }
        public DateTime VoteDateTime { get; set; } = DateTime.Now;



        public User User { get; set; }
        public Course Course { get; set; }
    }
}
