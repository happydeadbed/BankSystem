namespace BankApi.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int AccountId { get; set; }
        public BankAccount? Account { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal MonthlyAmount { get; set; }
        public int TotalMonths { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Active";
        public string CollateralDescription { get; set; } = string.Empty;
        public decimal CollateralValue { get; set; }
           public ICollection<LoanPayment> LoanPayments { get; set; } = new List<LoanPayment>();


    }
}
