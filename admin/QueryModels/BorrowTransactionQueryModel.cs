public class BorrowTransactionQueryModel
{
    public int TransactionId { get; set; }
    public string BookTitle { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }
    public decimal? FineAmount { get; set; }
    public string Status { get; set; }
}
