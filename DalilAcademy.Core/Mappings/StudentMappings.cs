using DalilAcademy.Core.DTOs.Students;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

// Lab 6 · Task 02 — ToDto() and ToEntity() extension methods
public static class StudentMappings
{
    public static StudentResponseDto ToDto(this Student student) => new(
        student.Id,
        student.FullName,
        student.Email);

    public static Student ToEntity(this CreateStudentDto dto) => new()
    {
        FullName = dto.FullName.Trim(),
        Email = dto.Email.Trim()
    };

    // Update: copy the editable fields onto the tracked entity
    public static void ApplyTo(this UpdateStudentDto dto, Student student)
    {
        student.FullName = dto.FullName.Trim();
        student.Email = dto.Email.Trim();
    }
}
