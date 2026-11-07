namespace DalilAcademy.Core.DTOs.Common;

// Lab 7 · Task 03 — never more than 50 per page (protects the server)
public static class PageRequest
{
    public const int MaxPageSize = 50;
    public const int DefaultPageSize = 10;

    public static (int Page, int PageSize) Normalize(int page, int pageSize) =>
        (Math.Max(page, 1), Math.Clamp(pageSize, 1, MaxPageSize));
}
