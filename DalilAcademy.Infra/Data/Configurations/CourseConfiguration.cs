using DalilAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DalilAcademy.Infra.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
               .IsRequired()
               .HasMaxLength(200);
        builder.HasIndex(c => c.Title).IsUnique();   // second line of defense for the Lab 3 rule

        builder.Property(c => c.Price).HasPrecision(10, 2);

        // Lab 4 · Task 04 — stored as text ("Beginner") so it's readable in pgAdmin
        builder.Property(c => c.Level)
               .HasConversion<string>()
               .HasMaxLength(20)
               .HasDefaultValue(CourseLevel.Beginner);

        builder.HasOne(c => c.Instructor)
               .WithMany(i => i.Courses)
               .HasForeignKey(c => c.InstructorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
