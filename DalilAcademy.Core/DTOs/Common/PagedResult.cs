namespace DalilAcademy.Core.DTOs.Common;

// One response with everything the front end needs for paging.
public record PagedResult<T>(List<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
