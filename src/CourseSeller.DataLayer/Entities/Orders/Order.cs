using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.DataLayer.Entities.Orders
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public string UserId { get; set; }


        [Required]
        public int OrderSum { get; set; }

        public bool IsFinished { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

        // This is for prevent use a discount twoice!
        public bool UsedDiscount { get; set; }


        public User User { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
