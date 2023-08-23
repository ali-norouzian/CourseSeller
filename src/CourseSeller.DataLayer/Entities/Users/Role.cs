using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Permissions;

namespace CourseSeller.DataLayer.Entities.Users
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Display(Name = "عنوان نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string RoleTitle { get; set; }

        public bool IsDelete { get; set; }


        #region Relations

        public List<UserRole> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }

        #endregion
    }
}
