using System.ComponentModel.DataAnnotations;

namespace CourseSeller.DataLayer.Entities.Courses;

public class CourseStatus
{
    [Key]
    public int StatusId { get; set; }

    [Required]
    [MaxLength(150)]
    public string StatusTitle { get; set; }


    public List<Course> Course { get; set; }
}