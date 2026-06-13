namespace BankApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public bool IsReversed { get; set; } = false;








    }
}
