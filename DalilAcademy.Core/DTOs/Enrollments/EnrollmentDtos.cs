namespace DalilAcademy.Core.DTOs.Enrollments;

public record CreateEnrollmentDto(int StudentId, int CourseId);

// flat: student and course names instead of nested objects
public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseTitle,
    DateTime EnrolledAt);
