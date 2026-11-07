using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.DTOs.Enrollments;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using FluentValidation;

namespace DalilAcademy.Infra.Service;

// One rule, three repositories.
public class EnrollmentService(
    IStudentRepository students,
    ICourseRepository courses,
    IEnrollmentRepository enrollments,
    IValidator<CreateEnrollmentDto> validator) : IEnrollmentService
{
    public async Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);                                       // -> 400

        var student = await students.GetByIdAsync(dto.StudentId)
            ?? throw new NotFoundException($"Student {dto.StudentId} was not found.");   // -> 404
        var course = await courses.GetByIdAsync(dto.CourseId)
            ?? throw new NotFoundException($"Course {dto.CourseId} was not found.");     // -> 404

        var enrollment = new Enrollment
        {
            StudentId = student.Id,
            CourseId = course.Id,
            EnrolledAt = DateTime.UtcNow                                                  // the UTC trap!
        };
        await enrollments.CreateAsync(enrollment);                                        // already enrolled -> 409
        return enrollment.ToDto(student, course);
    }

    // Lab 8 · Task 04 — GET /api/enrollments/me
    public async Task<List<CourseResponseDto>> GetCoursesForUserAsync(int userId) =>
        await students.GetCoursesByUserIdAsync(userId)
            ?? throw new NotFoundException("No student profile is linked to this account.");

    // POST /api/enrollments/me — a student enrolls THEMSELVES (student id from the token)
    public async Task<EnrollmentResponseDto> EnrollUserAsync(int userId, int courseId)
    {
        var studentId = await students.GetIdByUserIdAsync(userId)
            ?? throw new NotFoundException("No student profile is linked to this account.");
        return await EnrollAsync(new CreateEnrollmentDto(studentId, courseId));
    }
}
