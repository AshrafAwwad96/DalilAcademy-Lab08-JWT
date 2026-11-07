using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.DTOs.Students;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers;

// No error handling here: the service throws -> GlobalExceptionHandler -> ProblemDetails.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]   // Lab 8 · Task 03 — only admins create, list, update or delete students
public class StudentsController(IStudentService studentService) : ControllerBase
{
    // Lab 7 · Task 02 + 03 — GET /api/students?page=1&pageSize=10  (pageSize capped at 50)
    [HttpGet]
    public async Task<ActionResult<PagedResult<StudentResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize)
    {
        (page, pageSize) = PageRequest.Normalize(page, pageSize);
        return Ok(await studentService.GetPagedAsync(page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentResponseDto>> GetById(int id)
        => Ok(await studentService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> Create(CreateStudentDto dto)
    {
        var created = await studentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateStudentDto dto)
    { await studentService.UpdateAsync(id, dto); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    { await studentService.DeleteAsync(id); return NoContent(); }

    [HttpGet("{id:int}/courses")]
    public async Task<ActionResult<List<CourseResponseDto>>> GetCourses(int id)
        => Ok(await studentService.GetCoursesAsync(id));
}
