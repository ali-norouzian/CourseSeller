using System.ComponentModel.DataAnnotations;

namespace CourseSeller.DataLayer.Entities.Users
{
    public class UserRole
    {
        [Key]
        public int UR_Id { get; set; }

        public string UserId { get; set; }

        public int RoleId { get; set; }


        #region Relations

        public User User { get; set; }
        public Role Role { get; set; }

        #endregion
    }
}