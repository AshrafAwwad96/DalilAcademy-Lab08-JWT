using DalilAcademy.Core.DTOs.Common;
using DalilAcademy.Core.DTOs.Courses;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using FluentValidation;
using FluentValidation.Results;

namespace DalilAcademy.Infra.Service;

// No null / bool for failures any more: the service THROWS, GlobalExceptionHandler picks the status code.
public class CourseService(
    ICourseRepository courseRepository,
    IValidator<CreateCourseDto> createValidator,
    IValidator<UpdateCourseDto> updateValidator,
    IValidator<CoursePriceFilter> filterValidator) : ICourseService
{
    // GET endpoints: the repository projects straight into DTOs (no ToDto, no tracking)
    public async Task<PagedResult<CourseResponseDto>> GetPagedAsync(int page, int pageSize, CoursePriceFilter filter)
    {
        await filterValidator.ValidateAndThrowAsync(filter);                     // -> 400
        return await courseRepository.GetPagedAsync(page, pageSize, filter);
    }

    public async Task<CourseResponseDto> GetByIdAsync(int id) =>
        await courseRepository.GetDtoByIdAsync(id)
            ?? throw new NotFoundException($"Course {id} was not found.");      // -> 404

    public async Task<CountResponseDto> CountAsync(CoursePriceFilter filter)
    {
        await filterValidator.ValidateAndThrowAsync(filter);
        return new CountResponseDto(await courseRepository.CountAsync(filter));
    }

    public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);                        // -> 400
        await EnsureInstructorExistsAsync(dto.InstructorId);                     // -> 400
        await EnsureTitleIsFreeAsync(dto.Title);                                 // -> 409

        var created = await courseRepository.CreateAsync(dto.ToEntity());        // DB unique index -> 409
        return (await courseRepository.GetDtoByIdAsync(created.Id))!;            // projection fills InstructorName
    }

    public async Task UpdateAsync(int id, UpdateCourseDto dto)
    {
        var course = await courseRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Course {id} was not found.");      // -> 404
        await updateValidator.ValidateAndThrowAsync(dto);                        // -> 400
        await EnsureTitleIsFreeAsync(dto.Title, excludeId: id);                  // -> 409

        course.Title = dto.Title.Trim();
        course.Price = dto.Price;
        course.Level = dto.Level;
        await courseRepository.UpdateAsync(course);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await courseRepository.DeleteAsync(id))
            throw new NotFoundException($"Course {id} was not found.");         // -> 404
    }

    // PostgreSQL's unique index is case-sensitive, so check case-insensitively first.
    private async Task EnsureTitleIsFreeAsync(string title, int? excludeId = null)
    {
        if (await courseRepository.TitleExistsAsync(title, excludeId))
            throw new ConflictException($"A course with the title '{title}' already exists.");
    }

    // Needs the database -> business rule, but reported like a validation error on the field.
    private async Task EnsureInstructorExistsAsync(int instructorId)
    {
        if (!await courseRepository.InstructorExistsAsync(instructorId))
            throw new ValidationException([new ValidationFailure("InstructorId", $"Instructor {instructorId} does not exist.")]);
    }
}
