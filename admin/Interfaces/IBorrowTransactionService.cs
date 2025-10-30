using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.QueryModel;

namespace LibraryManagementSystem.Interfaces
{
    public interface IBorrowTransactionService
    {
        Task<List<BorrowTransactionQueryModel>> GetAllAsync();
        Task<BorrowTransactionQueryModel?> GetByIdAsync(int id);
        Task<bool> BorrowAsync(BorrowTransactionCommandModel model);
        Task<bool> ReturnAsync(BorrowTransactionReturnModel model);
    }

}
