namespace BankApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public string TransactionNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public int? FromAccountId { get; set; }

        public int? ToAccountId { get; set; }

        public decimal Amount { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public bool IsReversed { get; set; }
    }
}