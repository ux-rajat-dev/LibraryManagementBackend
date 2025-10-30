public class BorrowTransactionCommandModel
{
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateOnly BorrowDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly DueDate { get; set; } 
}
