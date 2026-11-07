using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IStudentRepository
{
    // ── Reads: projection ──
    Task<PagedResult<DalilAcademy.Core.DTOs.Students.StudentResponseDto>> GetPagedAsync(int page, int pageSize);
    Task<List<CourseResponseDto>?> GetCoursesAsync(int studentId);   // null = student not found
    Task<List<CourseResponseDto>?> GetCoursesByUserIdAsync(int userId);   // Lab 8 /me — null = no profile
    Task<int?> GetIdByUserIdAsync(int userId);

    // ── Writes / business logic ──
    Task<Student?> GetByIdAsync(int id);
    Task<Student> CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<bool> ExistsAsync(int id);
}
