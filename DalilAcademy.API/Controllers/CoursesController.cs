using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]                                   // every action needs a valid token...
public class CoursesController(ICourseService courseService) : ControllerBase
{
    // GET /api/courses?page=1&pageSize=10&minPrice=40&maxPrice=70
    [HttpGet]
    [AllowAnonymous]                          // ...except browsing the catalog
    public async Task<ActionResult<PagedResult<CourseResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        (page, pageSize) = PageRequest.Normalize(page, pageSize);     // pageSize = Math.Clamp(pageSize, 1, 50)
        return Ok(await courseService.GetPagedAsync(page, pageSize, new CoursePriceFilter(minPrice, maxPrice)));
    }

    // Lab 7 · Task 05 — GET /api/courses/count?minPrice=&maxPrice=
    [HttpGet("count")]
    [AllowAnonymous]
    public async Task<ActionResult<CountResponseDto>> Count(
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
        => Ok(await courseService.CountAsync(new CoursePriceFilter(minPrice, maxPrice)));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseResponseDto>> GetById(int id)
        => Ok(await courseService.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]                                  // only admins create courses
    public async Task<ActionResult<CourseResponseDto>> Create(CreateCourseDto dto)
    {
        var created = await courseService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Instructor}")]       // admin OR instructor
    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
    { await courseService.UpdateAsync(id, dto); return NoContent(); }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    { await courseService.DeleteAsync(id); return NoContent(); }
}
