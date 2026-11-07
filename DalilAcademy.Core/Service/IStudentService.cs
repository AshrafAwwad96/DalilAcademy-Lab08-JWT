using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.DTOs.Students;

namespace DalilAcademy.Core.Service;

public interface IStudentService
{
    Task<PagedResult<StudentResponseDto>> GetPagedAsync(int page, int pageSize);
    Task<StudentResponseDto> GetByIdAsync(int id);
    Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);
    Task UpdateAsync(int id, UpdateStudentDto dto);
    Task DeleteAsync(int id);
    Task<List<CourseResponseDto>> GetCoursesAsync(int studentId);
}
