using CourseSeller.DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.DataLayer.Contexts
{
    public class MssqlContext : DbContext
    {
        public MssqlContext(DbContextOptions<MssqlContext> options)
            : base(options) // pass opt to father class
        {
        }


        #region Users

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        #endregion



    }
}