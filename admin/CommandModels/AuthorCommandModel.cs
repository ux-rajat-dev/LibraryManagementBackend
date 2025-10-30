namespace LibraryManagementSystem.admin.CommandModels
{
    public class AuthorCommandModel
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
    }
}
