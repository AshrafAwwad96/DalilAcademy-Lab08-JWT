using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Repository;

public class UserRepository(AppDbContext context) : IUserRepository
{
    // emails are stored lower-case, so compare lower-case
    public Task<User?> GetByEmailAsync(string email)
        => context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email.ToLower());

    // a login OR an existing student profile with this email -> already taken
    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalized = email.ToLower();
        return await context.Users.AnyAsync(u => u.Email == normalized)
            || await context.Students.AnyAsync(s => s.Email.ToLower() == normalized);
    }

    public async Task<User> RegisterStudentAsync(User user, Student profile)
    {
        try
        {
            profile.User = user;                 // EF inserts the user first, then the student with its UserId
            context.Students.Add(profile);
            await context.SaveChangesAsync();    // one transaction: both rows or neither
            return user;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new ConflictException("This email is already registered.");   // -> 409
        }
    }
}
