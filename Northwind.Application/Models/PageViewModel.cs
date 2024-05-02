namespace Northwind.Application.Models
{
    public class PageViewModel
    {
        public int PageNumber { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public object? FkId { get; }
        public object? Pk2 { get; }

        public PageViewModel(int count, int pageNumber, int pageSize, object? fkId = null, object? pk2 = null)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            FkId = fkId;
            Pk2 = pk2;
        }
    }
}
