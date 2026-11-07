using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Repository;

public class EnrollmentRepository(AppDbContext db) : IEnrollmentRepository
{
    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        try
        {
            db.Enrollments.Add(enrollment);
            await db.SaveChangesAsync();
            return enrollment;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // unique index (StudentId, CourseId) -> 409 Conflict
            throw new ConflictException("The student is already enrolled in this course.");
        }
    }
}
