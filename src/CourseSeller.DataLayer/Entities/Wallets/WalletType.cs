using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseSeller.DataLayer.Entities.Wallets
{
    public class WalletType
    {
        // Not increment automatically
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(150)]
        public string TypeTitle { get; set; }


        #region Relations

        public List<Wallet> Wallets { get; set; }

        #endregion
    }
}