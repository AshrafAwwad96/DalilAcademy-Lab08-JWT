using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);

    // Creates the login AND its student profile in one SaveChanges (one transaction)
    Task<User> RegisterStudentAsync(User user, Student profile);
}
