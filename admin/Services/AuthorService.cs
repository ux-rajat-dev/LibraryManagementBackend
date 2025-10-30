using LibraryManagement.Models;
using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.admin.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly FreedbLibraryManagementContext _context;

        public AuthorService(FreedbLibraryManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAllAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task<bool> CreateAsync(AuthorCommandModel model)
        {
            var author = new Author
            {
                Name = model.Name,
                Bio = model.Bio
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(AuthorCommandModel model)
        {
            var author = await _context.Authors.FindAsync(model.AuthorId);
            if (author == null)
                return false;

            author.Name = model.Name;
            author.Bio = model.Bio;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
