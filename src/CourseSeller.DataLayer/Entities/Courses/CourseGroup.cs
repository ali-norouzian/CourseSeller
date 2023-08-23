using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseSeller.DataLayer.Entities.Courses
{
    public class CourseGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Display(Name = "عنوان گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string GroupTitle { get; set; }

        [Display(Name = "حذف شده")]
        public bool IsDelete { get; set; }

        [Display(Name = "گروه اصلی")]
        public int? ParentId { get; set; }


        #region Rel

        [ForeignKey("ParentId")]
        public List<CourseGroup> CourseGroups { get; set; }

        // Because we have two fk in the same shape like this. we must mapping inverse it too.
        // CourseGroup (in InverseProperty) is the name of CourseGroup class member object that related to this class.
        [InverseProperty("CourseGroup")]
        public List<Course> Course { get; set; }

        // Because we have two fk in the same shape like this. we must mapping inverse it too.
        // SubGroup (in InverseProperty) is the name of CourseGroup class member object that related to this class.
        [InverseProperty("SubGroup")]
        public List<Course> SubGroup { get; set; }


        #endregion
    }
}
