using LibraryManagement.Models;
using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.QueryModel;
using LibraryManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.admin.Services
{
    public class BookService : IBookService
    {
        private readonly FreedbLibraryManagementContext _context;

        public BookService(FreedbLibraryManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBookAsync(BookCreateCommandModel model)
        {
            var newBook = new Book
            {
                Title = model.Title,
                Description = model.Description,
                AuthorId = model.AuthorId,
                GenreId = model.GenreId,
                Isbn = model.Isbn,
                TotalCopies = model.TotalCopies,
                AvailableCopies = model.TotalCopies,
                PublishedYear = model.PublishedYear,
                CoverImageUrl = model.CoverImageUrl
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BookQueryModel>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Select(b => new BookQueryModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Description = b.Description,
                    AuthorName = b.Author != null ? b.Author.Name : null,
                    GenreName = b.Genre != null ? b.Genre.Name : null,
                    Isbn = b.Isbn,
                    TotalCopies = b.TotalCopies ?? 0,
                    AvailableCopies = b.AvailableCopies ?? 0,
                    PublishedYear = b.PublishedYear,
                    CoverImageUrl = b.CoverImageUrl
                })
                .ToListAsync();
        }

        public async Task<BookQueryModel?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return null;

            return new BookQueryModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                AuthorName = book.Author?.Name,
                GenreName = book.Genre?.Name,
                Isbn = book.Isbn,
                TotalCopies = book.TotalCopies ?? 0,
                AvailableCopies = book.AvailableCopies ?? 0,
                PublishedYear = book.PublishedYear,
                CoverImageUrl = book.CoverImageUrl
            };
        }

        public async Task<bool> UpdateBookAsync(BookUpdateCommandModel model)
        {
            var book = await _context.Books.FindAsync(model.BookId);

            if (book == null)
                return false;

            book.Title = model.Title;
            book.Description = model.Description;
            book.AuthorId = model.AuthorId;
            book.GenreId = model.GenreId;
            book.Isbn = model.Isbn;
            book.TotalCopies = model.TotalCopies;
            book.AvailableCopies = model.AvailableCopies;
            book.PublishedYear = model.PublishedYear;
            book.CoverImageUrl = model.CoverImageUrl;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }




    }
}
