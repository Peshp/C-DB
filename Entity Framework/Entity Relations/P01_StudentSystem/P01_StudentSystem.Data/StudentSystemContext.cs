using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext(DbContextOptions options)
            : base(options)
        { 

        }

        public DbSet<Student> Student { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(true);
                entity.Property(e => e.PhoneNumber).IsRequired(false).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.Birthday).IsRequired(false);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.Name).HasMaxLength(80).IsUnicode(true);
                entity.Property(e => e.Description).IsUnicode(true).IsRequired(false);
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);
                entity.Property(e => e.Name).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.Url).IsUnicode(false);

                entity.HasOne(e => e.Course).WithMany(e => e.Resources).HasForeignKey(e => e.CourseId);
            });

            modelBuilder.Entity<Homework>(entity => 
            {
                entity.HasKey(e => e.HomeworkId);
                entity.Property(e => e.Content).IsUnicode(false);

                entity.HasOne(e => e.Student).WithMany(e => e.Homeworks).HasForeignKey(e => e.StudentId);
                entity.HasOne(e => e.Course).WithMany(e => e.Homeworks).HasForeignKey(e => e.CourseId);
            });

            modelBuilder.Entity<StudentCourse>(entity => 
            { 
                entity.HasKey(e => new {e.StudentId, e.CourseId});

                entity.HasOne(e => e.Student).WithMany(e => e.StudentCourses).HasForeignKey(e => e.StudentId);
                entity.HasOne(e => e.Course).WithMany(e => e.StudentCourses).HasForeignKey(e => e.CourseId);
            });
        }
    }
}