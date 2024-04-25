namespace Northwind.Application.Models
{
    public class PageViewModel
    {
        public int PageNumber { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int FkId { get; }

        public PageViewModel(int count, int pageNumber, int pageSize, int fkId)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            FkId = fkId;
        }
    }
}
