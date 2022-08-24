using GameShop.DataLayer.Entities.Course;
using GameShop.DataLayer.Entities.Order;
using GameShop.DataLayer.Entities.Permission;
using GameShop.DataLayer.Entities.QustionAnswer;
using GameShop.DataLayer.Entities.User;
using GameShop.DataLayer.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameShop.DataLayer.Context
{
    public class GameShopContext : DbContext
    {
        public GameShopContext(DbContextOptions options) : base(options)
        {

        }

        #region User

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        #endregion

        #region Wallet

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletType> WalletTypes { get; set; }

        #endregion

        #region Permission

        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        #endregion

        #region Groups

        public DbSet<CourseGroup> CourseGroups { get; set; }

        #endregion

        #region Course

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseEpisode> CourseEpisodes { get; set; }

        public DbSet<CourseLevel> CourseLevels { get; set; }

        public DbSet<CourseStatus> CourseStatuses { get; set; }

        public DbSet<UserCourse> UserCourses { get; set; }

        public DbSet<CourseComment> CourseComments { get; set; }

        public DbSet<CourseVote> CourseVotes { get; set; }

        #endregion

        #region Order

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        public DbSet<UserDiscount> UserDiscounts { get; set; }

        #endregion

        #region Question & Answer

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        #endregion

        #region Overrides

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDelete);
            modelBuilder.Entity<CourseGroup>().HasQueryFilter(c => !c.IsDelete);
            modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDelete);

            modelBuilder.Entity<Course>()
                            .HasOne<CourseGroup>(c => c.CourseGroup)
                            .WithMany(g => g.Courses)
                            .HasForeignKey(f => f.GroupId)
                            .OnDelete(DeleteBehavior.NoAction);

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
    .SelectMany(t => t.GetForeignKeys())
    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<Course>()
    .HasOne<CourseGroup>(f => f.CourseGroup)
    .WithMany(g => g.Courses)
    .HasForeignKey(f => f.GroupId);

            modelBuilder.Entity<Course>()
                .HasOne<CourseGroup>(f => f.Group)
                .WithMany(g => g.SubGroup)
                .HasForeignKey(f => f.SubGroup);

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
