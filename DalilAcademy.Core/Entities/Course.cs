namespace DalilAcademy.Core.Entities;

// Plain C# class: zero attributes, zero packages.
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;   // Lab 4 · Task 04

    public int InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
    public List<Enrollment> Enrollments { get; set; } = [];
}
