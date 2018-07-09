using Project1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Project1.DAL
{
    public class ManagerContext : DbContext
    {
        public ManagerContext() : base("ManagerContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<User>()
                .HasMany<Role>(r => r.Roles)
                .WithMany(u => u.Users)
                .Map(ur =>
                {
                    ur.MapLeftKey("UserId");
                    ur.MapRightKey("RoleId");
                    ur.ToTable("UserRoles");
                });
            modelBuilder.Entity<User>()
                .HasMany<Permission>(p => p.Permissions)
                .WithMany(u => u.Users)
                .Map(ur =>
                {
                    ur.MapLeftKey("UserId");
                    ur.MapRightKey("PermissionId");
                    ur.ToTable("UserPermissions");
                });
            modelBuilder.Entity<Role>()
                .HasMany<Permission>(p => p.Permissions)
                .WithMany(r => r.Roles)
                .Map(ur =>
                {
                    ur.MapLeftKey("RoleId");
                    ur.MapRightKey("PermissionId");
                    ur.ToTable("RolesPermissions");
                });
            modelBuilder.Entity<User>()
                .HasMany<Product>(g => g.Products)
                .WithRequired(s => s.User)
                .HasForeignKey<int>(s => s.UserId);
            modelBuilder.Entity<User>()
               .HasMany<Post>(g => g.Posts)
               .WithRequired(s => s.User)
               .HasForeignKey<int>(s => s.UserId);
        }
    }
}