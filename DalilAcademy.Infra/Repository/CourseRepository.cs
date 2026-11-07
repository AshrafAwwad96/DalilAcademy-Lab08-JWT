using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Repository;

public class CourseRepository(AppDbContext db) : ICourseRepository
{
    // ───────────── Reads: AsNoTracking + Select() projection ─────────────

    // Lab 7 · Task 04 — filter in the database: Where() BEFORE CountAsync / ToListAsync
    private IQueryable<Course> Filtered(CoursePriceFilter filter)
    {
        var query = db.Courses.AsNoTracking();
        if (filter.MinPrice.HasValue) query = query.Where(c => c.Price >= filter.MinPrice.Value);
        if (filter.MaxPrice.HasValue) query = query.Where(c => c.Price <= filter.MaxPrice.Value);
        return query;
    }

    // Two queries: COUNT for the total + one page. Both run inside PostgreSQL.
    public async Task<PagedResult<CourseResponseDto>> GetPagedAsync(int page, int pageSize, CoursePriceFilter filter)
    {
        var query = Filtered(filter);
        var total = await query.CountAsync();                                  // query 1: the total

        var items = await query
            .OrderBy(c => c.Id)                                                // ALWAYS order before Skip
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseResponseDto(c.Id, c.Title, c.Price, c.Level, c.Instructor!.FullName))
            .ToListAsync();                                                    // query 2: one page

        return new PagedResult<CourseResponseDto>(items, page, pageSize, total);
    }

    public Task<CourseResponseDto?> GetDtoByIdAsync(int id) =>
        db.Courses.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto(c.Id, c.Title, c.Price, c.Level, c.Instructor!.FullName))
            .FirstOrDefaultAsync();                                            // 1 query, only 5 columns

    // Lab 7 · Task 05 — SELECT count(*) — one number comes back, not every row
    public Task<int> CountAsync(CoursePriceFilter filter) => Filtered(filter).CountAsync();

    // ───────────── Writes / business logic: tracked entities ─────────────

    public async Task<Course?> GetByIdAsync(int id) => await db.Courses.FindAsync(id);

    public async Task<Course> CreateAsync(Course course)
    {
        try
        {
            db.Courses.Add(course);
            await db.SaveChangesAsync();
            return course;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new ConflictException("A course with this title already exists.");   // -> 409
        }
    }

    public async Task UpdateAsync(Course course)
    {
        try
        {
            db.Courses.Update(course);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new ConflictException("A course with this title already exists.");   // -> 409
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return false;

        db.Courses.Remove(course);
        await db.SaveChangesAsync();
        return true;
    }

    // AnyAsync -> SELECT EXISTS(...) — stops at the first match
    public Task<bool> TitleExistsAsync(string title, int? excludeId = null)
    {
        var normalized = title.Trim().ToLower();
        return db.Courses.AnyAsync(c => c.Id != excludeId && c.Title.ToLower() == normalized);
    }

    public Task<bool> InstructorExistsAsync(int instructorId) =>
        db.Instructors.AnyAsync(i => i.Id == instructorId);
}
