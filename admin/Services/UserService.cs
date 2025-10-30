using LibraryManagement.Models;
using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.Admin.QueryModels;
using LibraryManagementSystem.Auth;
using LibraryManagementSystem.AuthModels;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserService : IUserService
{
    private readonly FreedbLibraryManagementContext _context;
    private readonly IConfiguration _configuration;

    public UserService(FreedbLibraryManagementContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<List<UserQueryModel>> GetAllAsync()
    {
        return await _context.Users
            .Select(u => new UserQueryModel
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserQueryModel?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        return new UserQueryModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> CreateAsync(UserCommandModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            return false;

        var passwordHash = new PasswordHasher<User>().HashPassword(null!, model.PasswordHash ?? "Default123!");

        var user = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = passwordHash,
            Role = model.Role ?? "User",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(int id, UserCommandModel model)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.Role = model.Role;

        if (!string.IsNullOrEmpty(model.PasswordHash))
        {
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, model.PasswordHash);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            return false;

        var passwordHasher = new PasswordHasher<User>();
        var hashedPassword = passwordHasher.HashPassword(null!, model.Password);

        var user = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = hashedPassword,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(LoginModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null) return null;

        var passwordVerification = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (passwordVerification != PasswordVerificationResult.Success)
            return null;

        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),           
                new Claim("role", user.Role ?? "User")                           
            };


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserQueryModel?> GetProfileAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        return new UserQueryModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
