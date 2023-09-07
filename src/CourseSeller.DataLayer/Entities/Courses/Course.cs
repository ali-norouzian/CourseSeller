using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseSeller.DataLayer.Entities.Orders;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.DataLayer.Entities.Courses;

public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Display(Name = "عنوان دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
    public string CourseTitle { get; set; }

    [Display(Name = "شرح دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
    public string CourseDescription { get; set; }

    [Display(Name = "قیمت دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
    public int CoursePrice { get; set; }

    [MaxLength(500)]
    public string Tags { get; set; }

    [MaxLength(50)]
    public string CourseImageName { get; set; }

    [MaxLength(100)]
    public string DemoFileName { get; set; }

    [Required]
    public DateTime CreateDateTime { get; set; }

    public DateTime? UpdateDateTime { get; set; }



    public int GroupId { get; set; }
    public int? SubGroupId { get; set; }
    public string TeacherId { get; set; }

    [Required]
    public int StatusId { get; set; }

    [Required]
    public int LevelId { get; set; }


    #region Rel

    [ForeignKey("TeacherId")]
    public User User { get; set; }


    /*
     * We have two same fk object like here.
     * It is there because we have group and sub group to handle.
     */
    // GroupId is in this class. it is for telling ef each same object is for what object.
    [ForeignKey("GroupId")]
    public CourseGroup CourseGroup { get; set; }

    // GroupId is in this class. it is for telling ef each same object is for what object.
    [ForeignKey("SubGroupId")]
    public CourseGroup SubGroup { get; set; }

    [ForeignKey("StatusId")]
    public CourseStatus CourseStatus { get; set; }

    [ForeignKey("LevelId")]
    public CourseLevel CourseLevel { get; set; }

    public List<CourseEpisode> CourseEpisodes { get; set; }


    public List<OrderDetail> OrderDetails { get; set; }


    public List<UserCourse> UserCourses { get; set; }

    #endregion
}