using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Courses;
using CourseSeller.DataLayer.Entities.Orders;
using CourseSeller.DataLayer.Entities.Wallets;


namespace CourseSeller.DataLayer.Entities.Users
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        [MaxLength(32, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string UserId { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد.")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string Password { get; set; }

        [Display(Name = "کد فعالسازی")]
        [MaxLength(64, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string ActiveCode { get; set; }

        [Display(Name = "تاریخ تولید کد فعالسازی")]
        public DateTime ActiveCodeGenerateDateTime { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        [Display(Name = "آواتار")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string UserAvatar { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegisterDateTime { get; set; }

        [Display(Name = "موجودی حساب")]
        public int WalletBalance { get; set; } = 0;

        [Display(Name = "حذف شده")]
        public bool IsDelete { get; set; }


        #region Relations

        public List<UserRole> UserRoles { get; set; }
        public List<Wallet> Wallets { get; set; }
        public List<Course> Courses { get; set; }
        public List<Order> Orders { get; set; }
        public List<UserCourse> UserCourses { get; set; }
        public List<Comment> Comments { get; set; }

        #endregion
    }
}