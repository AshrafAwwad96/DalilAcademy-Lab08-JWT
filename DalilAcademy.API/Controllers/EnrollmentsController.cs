using System.Security.Claims;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.DTOs.Enrollments;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers;

public record EnrollMeDto(int CourseId);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
{
    // The id comes from the SIGNED token — the caller cannot change it.
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Admin enrolls any student: POST /api/enrollments { studentId, courseId }
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<EnrollmentResponseDto>> Enroll(CreateEnrollmentDto dto)
    {
        var enrollment = await enrollmentService.EnrollAsync(dto);
        return Created($"/api/students/{enrollment.StudentId}/courses", enrollment);
    }

    // Lab 8 · Task 04 — GET /api/enrollments/me -> the logged-in student's courses
    [HttpGet("me")]
    [Authorize(Roles = Roles.Student)]
    public async Task<ActionResult<List<CourseResponseDto>>> MyCourses()
        => Ok(await enrollmentService.GetCoursesForUserAsync(CurrentUserId));

    // A student enrolls themselves: POST /api/enrollments/me { courseId }
    [HttpPost("me")]
    [Authorize(Roles = Roles.Student)]
    public async Task<ActionResult<EnrollmentResponseDto>> EnrollMe(EnrollMeDto dto)
    {
        var enrollment = await enrollmentService.EnrollUserAsync(CurrentUserId, dto.CourseId);
        return Created("/api/enrollments/me", enrollment);
    }
}
