using DalilAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Finds every IEntityTypeConfiguration<T> in this project
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {
            // Lab 4 · Task 03 — students have their own check, placed BEFORE the instructor check's return
            if (!context.Set<Student>().Any())
            {
                context.Set<Student>().AddRange(
                    new Student { FullName = "Omar Khalil",  Email = "omar.khalil@dalil.academy" },
                    new Student { FullName = "Sara Nasser",  Email = "sara.nasser@dalil.academy" },
                    new Student { FullName = "Yousef Ali",   Email = "yousef.ali@dalil.academy" });
                context.SaveChanges();
            }

            // Lecture 8 — the first logins. BEFORE the instructor check's return, or they'd never be created.
            // "if" because seeding runs on every update and Email is unique.
            if (!context.Set<User>().Any())
            {
                context.Set<User>().AddRange(
                    new User { Email = "admin@dalil.jo", Role = Roles.Admin,
                               PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123") },
                    new User { Email = "instructor@dalil.jo", Role = Roles.Instructor,
                               PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor@123") });
                context.SaveChanges();
            }

            if (context.Set<Instructor>().Any()) return;   // seed only once

            var ahmad = new Instructor { FullName = "Ahmad Saleh" };
            var lina  = new Instructor { FullName = "Lina Haddad" };

            context.Set<Course>().AddRange(
                new Course { Title = "C# Fundamentals",       Price = 50, Instructor = ahmad },
                new Course { Title = "ASP.NET Core Web API",  Price = 80, Instructor = ahmad },
                new Course { Title = "PostgreSQL Essentials", Price = 60, Instructor = lina });
            context.SaveChanges();
        });
    }
}
