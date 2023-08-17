using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.DataLayer.Entities.Wallets
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        [Display(Name = "نوع تراکنش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public int TypeId { get; set; }

        [Display(Name = "کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string UserId { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public int Amount { get; set; }

        [Display(Name = "پرداخت شده")]
        public bool IsPaid { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "تاریخ و ساعت")]
        public DateTime CreatedDateTime { get; set; }


        #region Relations

        public User User { get; set; }
        public WalletType WalletType { get; set; }

        #endregion
    }
}