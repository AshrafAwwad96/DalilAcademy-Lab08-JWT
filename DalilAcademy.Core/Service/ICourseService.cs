using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;

namespace DalilAcademy.Core.Service;

public interface ICourseService
{
    Task<PagedResult<CourseResponseDto>> GetPagedAsync(int page, int pageSize, CoursePriceFilter filter);
    Task<CourseResponseDto> GetByIdAsync(int id);
    Task<CountResponseDto> CountAsync(CoursePriceFilter filter);
    Task<CourseResponseDto> CreateAsync(CreateCourseDto dto);
    Task UpdateAsync(int id, UpdateCourseDto dto);
    Task DeleteAsync(int id);
}
