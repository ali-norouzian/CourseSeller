using System.ComponentModel.DataAnnotations;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.DataLayer.Entities.Permissions
{
    public class RolePermission
    {
        [Key]
        public int RP_Id { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }


        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
