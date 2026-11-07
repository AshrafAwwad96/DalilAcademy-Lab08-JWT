using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IEnrollmentRepository
{
    Task<Enrollment> CreateAsync(Enrollment enrollment);
}
