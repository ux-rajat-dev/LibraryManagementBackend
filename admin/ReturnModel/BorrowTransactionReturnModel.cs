public class BorrowTransactionReturnModel
{
    public int TransactionId { get; set; }
    public DateOnly ReturnDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}
