using CourseSeller.DataLayer.Entities.Courses;
using CourseSeller.DataLayer.Entities.Orders;
using CourseSeller.DataLayer.Entities.Permissions;
using CourseSeller.DataLayer.Entities.Users;
using CourseSeller.DataLayer.Entities.Wallets;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.DataLayer.Contexts
{
    public class MssqlContext : DbContext
    {
        public MssqlContext(DbContextOptions<MssqlContext> options)
            : base(options) // pass opt to father class
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Only query in this users
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.IsDelete == false);
            modelBuilder.Entity<Role>()
                .HasQueryFilter(r => r.IsDelete == false);
            modelBuilder.Entity<CourseGroup>()
                .HasQueryFilter(cg => cg.IsDelete == false);

            base.OnModelCreating(modelBuilder);
        }


        #region Users

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        #endregion


        #region Wallet

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletType> WalletTypes { get; set; }

        #endregion


        #region Permission

        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }

        #endregion


        #region Courses

        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<CourseLevel> CourseLevels { get; set; }
        public DbSet<CourseStatus> CourseStatus { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEpisode> CourseEpisodes { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }

        #endregion


        #region Orders

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        #endregion
    }
}