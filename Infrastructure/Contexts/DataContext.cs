using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<SubscriberEntity> Subscribers { get; set; }
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ContactEntity> Contacts { get; set; }
    public DbSet<SavedCourseEntity> SavedCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SavedCourseEntity>()
            .HasKey(sc => new { sc.UserId, sc.CourseId });

        modelBuilder.Entity<SavedCourseEntity>()
            .HasOne(sc => sc.User)
            .WithMany(u => u.SavedCourses)
            .HasForeignKey(sc => sc.UserId)
            .IsRequired();

        modelBuilder.Entity<SavedCourseEntity>()
            .HasOne(sc => sc.Course)
            .WithMany(c => c.SavedCourses)
            .HasForeignKey(sc => sc.CourseId)
            .IsRequired();
    }

}

