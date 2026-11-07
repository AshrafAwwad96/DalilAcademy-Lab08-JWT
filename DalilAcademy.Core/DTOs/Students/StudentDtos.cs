namespace DalilAcademy.Core.DTOs.Students;

// Lab 6 · Task 01 — Create, Update and Response records
public record CreateStudentDto(string FullName, string Email);   // no Id: over-posting protection

public record UpdateStudentDto(string FullName, string Email);

public record StudentResponseDto(int Id, string FullName, string Email);   // no Enrollments graph
