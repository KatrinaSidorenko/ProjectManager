using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ProjectManagerContext : IdentityDbContext<AppUser>
    {
        public ProjectManagerContext(DbContextOptions<ProjectManagerContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
               .HasMany<Assigment>(a => a.Assigments)
               .WithOne(p => p.AssigmentProject)
               .HasForeignKey(p => p.CurrentProjectId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasOne<AppUser>(u => u.ProjectOwner)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(a => a.CurrentProjectOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Assigment>()
                .HasMany<AssigmentFile>(p => p.AssigmentFiles)
                .WithOne(p => p.Assigment)
                .HasForeignKey(a => a.CurrentAssigmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assigment>()
                .HasMany<UserAssigment>(f => f.UserAssigments)
                .WithOne(a => a.Assigment)
                .HasForeignKey(a => a.AssigmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAssigment>().HasKey(sc => new { sc.AssigmentId, sc.UserId });
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Assigment> Assigments { get; set; }
        public DbSet<UserAssigment> UserAssigments { get; set; }
        public DbSet<AssigmentFile> AssigmentFiles { get; set; }

    }
}
