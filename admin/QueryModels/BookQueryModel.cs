namespace LibraryManagementSystem.admin.QueryModel
{
    public class BookQueryModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? AuthorName { get; set; }
        public string? GenreName { get; set; }
        public string? Isbn { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int? PublishedYear { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
