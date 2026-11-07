using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

public static class CourseMappings
{
    public static CourseResponseDto ToDto(this Course course) => new(
        course.Id,
        course.Title,
        course.Price,
        course.Level,
        course.Instructor?.FullName);   // filled only when the repository Include()s the Instructor

    public static Course ToEntity(this CreateCourseDto dto) => new()
    {
        Title = dto.Title.Trim(),
        Price = dto.Price,
        Level = dto.Level,
        InstructorId = dto.InstructorId
    };
}
