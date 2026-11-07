using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.DTOs.Enrollments;

namespace DalilAcademy.Core.Service;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto);

    // Lab 8 · Task 04 — the logged-in student (id from the signed token)
    Task<List<CourseResponseDto>> GetCoursesForUserAsync(int userId);
    Task<EnrollmentResponseDto> EnrollUserAsync(int userId, int courseId);
}
