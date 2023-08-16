using System.ComponentModel.DataAnnotations;

namespace CourseSeller.Core.DTOs.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ActiveCode { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        [Compare("Password", ErrorMessage = "کلمه های عبور مغایرت دارند.")]
        public string RePassword { get; set; }
    }

}