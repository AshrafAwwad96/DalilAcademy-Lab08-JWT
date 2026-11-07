namespace DalilAcademy.Core.DTOs.Courses;

// Lab 7 · Task 04 — optional price range, applied with Where() inside PostgreSQL
public record CoursePriceFilter(decimal? MinPrice, decimal? MaxPrice);

public record CountResponseDto(int Count);
