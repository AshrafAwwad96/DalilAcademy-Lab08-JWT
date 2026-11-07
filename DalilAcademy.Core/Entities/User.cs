namespace DalilAcademy.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;   // never the real password
    public string Role { get; set; } = Roles.Student;
}
