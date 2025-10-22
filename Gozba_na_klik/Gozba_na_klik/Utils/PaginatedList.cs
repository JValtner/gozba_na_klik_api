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

            // calculate total pages first
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // normalize page index (1-based externally, 0-based internally if you prefer)
            PageIndex = Math.Max(1, pageIndex); // enforce minimum of 1
            if (TotalPages > 0 && PageIndex > TotalPages)
                PageIndex = TotalPages;

            HasPreviousPage = PageIndex > 1;
            HasNextPage = PageIndex < TotalPages;
        }
    }
}