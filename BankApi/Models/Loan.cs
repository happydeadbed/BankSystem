namespace BankApi.Models
{
    public class Loan
    {
        public int Id { get; set; }

        public string LoanNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public decimal Amount { get; set; }

        public decimal InterestRate { get; set; }

        public int DurationMonths { get; set; }

        public DateTime StartDate { get; set; }

        public string Status { get; set; } = "Active";

        public ICollection<LoanPayment> Payments { get; set; } = new List<LoanPayment>();
    }
}