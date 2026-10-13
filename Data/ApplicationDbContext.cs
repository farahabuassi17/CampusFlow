
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CampusFlow.Mvc.Models;
using CampusFlow.Mvc.Models.Enums;

namespace CampusFlow.Mvc.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
            .Property(x => x.LoginProvider)
            .HasMaxLength(128);

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
            .Property(x => x.ProviderKey)
            .HasMaxLength(128);

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
            .Property(x => x.LoginProvider)
            .HasMaxLength(128);

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
            .Property(x => x.Name)
            .HasMaxLength(128);

        modelBuilder.Entity<User>()
            .Property(u => u.UserType)
            .HasDefaultValue(UserType.User);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Course)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
