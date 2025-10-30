namespace LibraryManagementSystem.admin.QueryModel
{
    public class AuthorQueryModel
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = null!;

        public string? Bio { get; set; }
    }
}
