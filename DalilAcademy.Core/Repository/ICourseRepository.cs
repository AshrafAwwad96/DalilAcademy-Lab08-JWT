using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface ICourseRepository
{
    // ── Reads: projection straight into DTOs (AsNoTracking) ──
    Task<PagedResult<CourseResponseDto>> GetPagedAsync(int page, int pageSize, CoursePriceFilter filter);
    Task<CourseResponseDto?> GetDtoByIdAsync(int id);
    Task<int> CountAsync(CoursePriceFilter filter);

    // ── Writes / business logic: tracked entities ──
    Task<Course?> GetByIdAsync(int id);
    Task<Course> CreateAsync(Course course);
    Task UpdateAsync(Course course);
    Task<bool> DeleteAsync(int id);
    Task<bool> TitleExistsAsync(string title, int? excludeId = null);
    Task<bool> InstructorExistsAsync(int instructorId);
}
