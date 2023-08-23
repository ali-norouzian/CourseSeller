using System.ComponentModel.DataAnnotations;

namespace CourseSeller.DataLayer.Entities.Courses;

public class CourseLevel
{
    [Key]
    public int LevelId { get; set; }

    [MaxLength(150)]
    [Required]
    public string LevelTitle { get; set; }


    public List<Course> Course { get; set; }
}