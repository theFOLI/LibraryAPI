namespace LibraryAPI.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverImagePath { get; set; }
        public string Sinopse { get; set; }
        public int PageNumber { get; set; }
        public int Year { get; set; }
        public string Editora { get; set; }
    }
}
