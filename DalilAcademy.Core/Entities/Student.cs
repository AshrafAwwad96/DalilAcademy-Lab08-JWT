namespace DalilAcademy.Core.Entities;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Enrollment> Enrollments { get; set; } = [];

    // Lab 8 · Task 02 — link the profile to a login so /me can find it
    public int? UserId { get; set; }
    public User? User { get; set; }
}
