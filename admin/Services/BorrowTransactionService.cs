using LibraryManagement.Models;
using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.QueryModel;
using LibraryManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

public class BorrowTransactionService : IBorrowTransactionService
{
    private readonly FreedbLibraryManagementContext _context;

    public BorrowTransactionService(FreedbLibraryManagementContext context)
    {
        _context = context;
    }

    public async Task<List<BorrowTransactionQueryModel>> GetAllAsync()
    {
        var transactions = await _context.Borrowtransactions
            .Include(bt => bt.Book)
            .Include(bt => bt.User)
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);

        var result = transactions.Select(bt =>
        {
            decimal? fine = bt.FineAmount; // Start with existing fine if any

            // Calculate fine dynamically
            if (bt.DueDate.HasValue)
            {
                var dueDate = bt.DueDate.Value;
                DateOnly effectiveDate;

                if (bt.ReturnDate.HasValue)
                {
                    // Book has been returned — use ReturnDate
                    effectiveDate = bt.ReturnDate.Value;
                }
                else
                {
                    // Book not returned — use today's date
                    effectiveDate = today;
                }

                // If overdue, calculate fine
                if (effectiveDate > dueDate)
                {
                    int daysLate = effectiveDate.DayNumber - dueDate.DayNumber;
                    fine = daysLate * 20; // ₹20 per day
                }
                else
                {
                    fine = 0;
                }
            }

            return new BorrowTransactionQueryModel
            {
                TransactionId = bt.TransactionId,
                BookTitle = bt.Book?.Title ?? "",
                UserEmail = bt.User?.Email ?? "",
                BorrowDate = bt.BorrowDate ?? default,
                DueDate = bt.DueDate ?? default,
                ReturnDate = bt.ReturnDate,
                FineAmount = fine,
                Status = bt.Status
            };
        }).ToList();

        return result;
    }


    public async Task<BorrowTransactionQueryModel?> GetByIdAsync(int id)
    {
        var bt = await _context.Borrowtransactions
            .Include(x => x.Book)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TransactionId == id);

        if (bt == null) return null;

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (bt.ReturnDate == null &&
            bt.DueDate.HasValue &&
            today > bt.DueDate.Value &&
            (bt.FineAmount == null || bt.FineAmount == 0))
        {
            var overdueDays = (today.ToDateTime(TimeOnly.MinValue) - bt.DueDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
            bt.FineAmount = overdueDays * 20;
            await _context.SaveChangesAsync();
        }

        return new BorrowTransactionQueryModel
        {
            TransactionId = bt.TransactionId,
            BookTitle = bt.Book?.Title ?? "",
            UserEmail = bt.User?.Email ?? "",
            BorrowDate = bt.BorrowDate ?? default,
            DueDate = bt.DueDate ?? default,
            ReturnDate = bt.ReturnDate,
            FineAmount = bt.FineAmount,
            Status = bt.Status
        };
    }

    public async Task<bool> BorrowAsync(BorrowTransactionCommandModel model)
    {
        var book = await _context.Books.FindAsync(model.BookId);
        if (book == null || book.AvailableCopies <= 0)
            return false;

        book.AvailableCopies--;

        var transaction = new Borrowtransaction
        {
            UserId = model.UserId,
            BookId = model.BookId,
            BorrowDate = model.BorrowDate,
            DueDate = model.DueDate,
            Status = "borrowed",
            
        };

        _context.Borrowtransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReturnAsync(BorrowTransactionReturnModel model)
    {
        var transaction = await _context.Borrowtransactions
            .Include(x => x.Book)
            .FirstOrDefaultAsync(x => x.TransactionId == model.TransactionId);

        if (transaction == null || transaction.ReturnDate != null)
            return false;

        // Set return date and status
        transaction.ReturnDate = model.ReturnDate;
        transaction.Status = "returned";

        // Increment available copies
        if (transaction.Book != null)
        {
            transaction.Book.AvailableCopies++;
        }

        // Calculate fine
        if (transaction.DueDate.HasValue)
        {
            // Convert DateOnly to DateTime for safe subtraction
            var dueDateTime = transaction.DueDate.Value.ToDateTime(TimeOnly.MinValue);
            var returnDateTime = model.ReturnDate.ToDateTime(TimeOnly.MinValue);

            int overdueDays = (returnDateTime - dueDateTime).Days;
            transaction.FineAmount = overdueDays > 0 ? overdueDays * 20 : 0;
        }
        else
        {
            transaction.FineAmount = 0;
        }

        await _context.SaveChangesAsync();
        return true;
    }


}
