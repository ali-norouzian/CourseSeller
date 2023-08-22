using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CourseSeller.Core.DTOs.Admin
{
    public class EditUserViewModel
    {
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

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public string AvatarName { get; set; }

        [Display(Name = "آواتار")]
        public IFormFile Avatar { get; set; }

        [Display(Name = "نقش ها")]
        public List<int> SelectedRoles { get; set; }

    }
}
