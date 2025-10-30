using LibraryManagement.Models;
using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.AuthModels;

namespace LibraryManagementSystem.admin.Interfaces
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllAsync();
        Task<Genre?> GetByIdAsync(int id);
        Task<bool> CreateAsync(GenreCommandModel model);
        Task<bool> UpdateAsync(GenreCommandModel model);
        Task<bool> DeleteAsync(int id);
    }
}
