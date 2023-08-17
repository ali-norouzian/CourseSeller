using System.ComponentModel.DataAnnotations;

namespace CourseSeller.Core.DTOs.UserPanel.Wallet;

public class ChargeWalletViewModel
{
    [Display(Name = "مبلغ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
    public int Amount { get; set; }
}
