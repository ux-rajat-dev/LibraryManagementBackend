namespace LibraryManagementSystem.admin.CommandModels;

public class UserCommandModel
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Role { get; set; }
}
