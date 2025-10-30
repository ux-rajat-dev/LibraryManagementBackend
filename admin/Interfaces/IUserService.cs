using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.Admin.QueryModels;
using LibraryManagementSystem.Auth;
using LibraryManagementSystem.AuthModels;

public interface IUserService
{
    Task<List<UserQueryModel>> GetAllAsync();
    Task<UserQueryModel?> GetByIdAsync(int id);
    Task<bool> CreateAsync(UserCommandModel model);
    Task<bool> UpdateAsync(int id, UserCommandModel model);
    Task<bool> DeleteAsync(int id);
    Task<string?> LoginAsync(LoginModel model);
    Task<bool> RegisterAsync(RegisterModel model);
    Task<UserQueryModel?> GetProfileAsync(string email);
}
