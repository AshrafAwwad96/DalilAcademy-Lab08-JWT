using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.DTOs.Students;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using FluentValidation;

namespace DalilAcademy.Infra.Service;

// Lab 6 · Task 04 — NotFound and Conflict exceptions instead of null / bool
public class StudentService(
    IStudentRepository studentRepository,
    IValidator<CreateStudentDto> createValidator,
    IValidator<UpdateStudentDto> updateValidator) : IStudentService
{
    public Task<PagedResult<StudentResponseDto>> GetPagedAsync(int page, int pageSize) =>
        studentRepository.GetPagedAsync(page, pageSize);

    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await studentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");     // -> 404
        return student.ToDto();
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);                        // -> 400
        await EnsureEmailIsFreeAsync(dto.Email);                                 // -> 409

        var created = await studentRepository.CreateAsync(dto.ToEntity());       // DB unique index -> 409
        return created.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateStudentDto dto)
    {
        var student = await studentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");     // -> 404
        await updateValidator.ValidateAndThrowAsync(dto);                        // -> 400
        await EnsureEmailIsFreeAsync(dto.Email, excludeId: id);                  // -> 409

        dto.ApplyTo(student);
        await studentRepository.UpdateAsync(student);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await studentRepository.DeleteAsync(id))
            throw new NotFoundException($"Student {id} was not found.");        // -> 404
    }

    public async Task<List<CourseResponseDto>> GetCoursesAsync(int studentId)
    {
        // already projected into DTOs by the repository — 1 SQL query
        return await studentRepository.GetCoursesAsync(studentId)
            ?? throw new NotFoundException($"Student {studentId} was not found.");
    }

    private async Task EnsureEmailIsFreeAsync(string email, int? excludeId = null)
    {
        if (await studentRepository.EmailExistsAsync(email, excludeId))
            throw new ConflictException($"A student with the email '{email}' already exists.");
    }
}
