namespace LibraryManagementSystem.Admin.QueryModels;

public class UserQueryModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Role { get; set; }
    public DateTime? CreatedAt { get; set; }
}
