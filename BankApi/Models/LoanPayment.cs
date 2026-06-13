namespace BankApi.Models
{
    public class LoanPayment
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public Loan? loan { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
