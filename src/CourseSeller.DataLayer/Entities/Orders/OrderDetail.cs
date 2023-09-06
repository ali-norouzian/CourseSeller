using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Courses;

namespace CourseSeller.DataLayer.Entities.Orders
{
    public class OrderDetail
    {
        [Key]
        public int DetailId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int CourseId { get; set; }


        [Required]
        public int CurrentPrice { get; set; }
        [Required]
        public int Count { get; set; }


        public Order Order { get; set; }
        public Course Course { get; set; }

    }
}
