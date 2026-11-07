namespace DalilAcademy.Core.Entities;

public class Instructor
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public List<Course> Courses { get; set; } = [];
}
