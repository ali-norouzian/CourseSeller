using System.ComponentModel.DataAnnotations;

namespace CourseSeller.DataLayer.Entities.Orders
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Code { get; set; }

        [Required]
        public int Percentage { get; set; }

        public int? UsableCount { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

    }
}
