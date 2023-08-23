﻿using CourseSeller.DataLayer.Entities.Users;
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

    }
}