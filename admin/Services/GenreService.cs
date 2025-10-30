using LibraryManagement.Models;
using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.admin.Services
{
    public class GenreService : IGenreService
    {
        private readonly FreedbLibraryManagementContext _context;

        public GenreService(FreedbLibraryManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<bool> CreateAsync(GenreCommandModel model)
        {
            var genre = new Genre
            {
                Name = model.Name
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(GenreCommandModel model)
        {
            var genre = await _context.Genres.FindAsync(model.GenreId);
            if (genre == null)
                return false;

            genre.Name = model.Name;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return false;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
