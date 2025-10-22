namespace Gozba_na_klik.Utils
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int Count { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            Items = items;
            Count = count;
            PageIndex = pageIndex;
            PageIndex = Math.Min(Math.Max(0, pageIndex), TotalPages - 1);
            HasPreviousPage = PageIndex > 0;
            HasNextPage = PageIndex < TotalPages - 1;
        }
    }

}
