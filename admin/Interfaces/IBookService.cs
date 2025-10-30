using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.QueryModel;

public interface IBookService
{
    Task<bool> CreateBookAsync(BookCreateCommandModel model);
    Task<List<BookQueryModel>> GetAllBooksAsync();
    Task<BookQueryModel?> GetBookByIdAsync(int id);
    Task<bool> UpdateBookAsync(BookUpdateCommandModel model);
    Task<bool> DeleteBookAsync(int id);



}
