using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.DTOs.Students;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Repository;

public class StudentRepository(AppDbContext db) : IStudentRepository
{
    // ───────────── Reads ─────────────

    // Lab 7 · Task 02 — PagedResult<T>: COUNT + one ordered page
    public async Task<PagedResult<StudentResponseDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = db.Students.AsNoTracking();
        var total = await query.CountAsync();                                  // query 1

        var items = await query
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new StudentResponseDto(s.Id, s.FullName, s.Email))
            .ToListAsync();                                                    // query 2

        return new PagedResult<StudentResponseDto>(items, page, pageSize, total);
    }

    // Lab 7 · Task 01 — a student's courses in ONE query.
    // Projection from the student row: null = no such student, [] = student with no courses.
    public async Task<List<CourseResponseDto>?> GetCoursesAsync(int studentId) =>
        await db.Students.AsNoTracking()
            .Where(s => s.Id == studentId)
            .Select(s => s.Enrollments
                .OrderBy(e => e.EnrolledAt)
                .Select(e => new CourseResponseDto(
                    e.Course!.Id, e.Course.Title, e.Course.Price, e.Course.Level, e.Course.Instructor!.FullName))
                .ToList())
            .FirstOrDefaultAsync();

    // Lab 8 · Task 04 — /me: find the profile by the user id from the token, 1 query
    public async Task<List<CourseResponseDto>?> GetCoursesByUserIdAsync(int userId) =>
        await db.Students.AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => s.Enrollments
                .OrderBy(e => e.EnrolledAt)
                .Select(e => new CourseResponseDto(
                    e.Course!.Id, e.Course.Title, e.Course.Price, e.Course.Level, e.Course.Instructor!.FullName))
                .ToList())
            .FirstOrDefaultAsync();

    public Task<int?> GetIdByUserIdAsync(int userId) =>
        db.Students.Where(s => s.UserId == userId).Select(s => (int?)s.Id).FirstOrDefaultAsync();

    // ───────────── Writes / business logic ─────────────

    public async Task<Student?> GetByIdAsync(int id) => await db.Students.FindAsync(id);

    public Task<bool> ExistsAsync(int id) => db.Students.AnyAsync(s => s.Id == id);

    public async Task<Student> CreateAsync(Student student)
    {
        try
        {
            db.Students.Add(student);
            await db.SaveChangesAsync();
            return student;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new ConflictException("A student with this email already exists.");   // -> 409
        }
    }

    public async Task UpdateAsync(Student student)
    {
        try
        {
            db.Students.Update(student);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new ConflictException("A student with this email already exists.");   // -> 409
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await db.Students.FindAsync(id);
        if (student is null) return false;

        db.Students.Remove(student);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var normalized = email.Trim().ToLower();
        return db.Students.AnyAsync(s => s.Id != excludeId && s.Email.ToLower() == normalized);
    }
}
