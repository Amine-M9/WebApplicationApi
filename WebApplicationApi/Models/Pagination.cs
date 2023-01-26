namespace WebApplicationApi.Models
{
    public class Pagination
    {
        public int TotalFilms { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public Pagination()
        {
        }
        public Pagination(int totalFilms,int page, int pageSize=8)
        {
            int totalPages =  (int)Math.Ceiling((decimal)totalFilms/ (decimal)pageSize);
            int currentPage = page;
            int startPage = currentPage - 5;
            int endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage = endPage- (startPage-1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 8)
                {
                    startPage = endPage - 7;
                }
            }
            TotalFilms= totalFilms;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;

        }

    }

    
}
