using DalilAcademy.Core.DTOs.Enrollments;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

public static class EnrollmentMappings
{
    public static EnrollmentResponseDto ToDto(this Enrollment e, Student student, Course course) => new(
        e.Id,
        student.Id,
        student.FullName,
        course.Id,
        course.Title,
        e.EnrolledAt);
}
