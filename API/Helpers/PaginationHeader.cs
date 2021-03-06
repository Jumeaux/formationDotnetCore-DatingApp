namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems,int totalPages)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalPages = totalPages;
            this.TotalItems = totalItems;
        }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }

        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}