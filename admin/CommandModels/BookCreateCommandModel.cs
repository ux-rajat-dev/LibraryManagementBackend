namespace LibraryManagementSystem.admin.CommandModels
{
    public class BookCreateCommandModel
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? AuthorId { get; set; }
        public int? GenreId { get; set; }
        public string? Isbn { get; set; }
        public int? TotalCopies { get; set; }
        public int? PublishedYear { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
