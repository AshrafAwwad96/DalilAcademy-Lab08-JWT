using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.DTOs.Courses;

// Records: one line each, immutable. Inputs carry only what the client may send.
public record CreateCourseDto(string Title, decimal Price, int InstructorId, CourseLevel Level = CourseLevel.Beginner);

public record UpdateCourseDto(string Title, decimal Price, CourseLevel Level = CourseLevel.Beginner);

public record CourseResponseDto(
    int Id,
    string Title,
    decimal Price,
    CourseLevel Level,
    string? InstructorName);   // flattened — no nested Instructor object
