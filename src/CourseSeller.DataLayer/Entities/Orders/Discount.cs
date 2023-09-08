using System.ComponentModel.DataAnnotations;

namespace CourseSeller.DataLayer.Entities.Orders
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Display(Name = "کد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(100)]
        public string Code { get; set; }

        [Display(Name = "درصد تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public int Percentage { get; set; }

        public int? UsableCount { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

    }
}
