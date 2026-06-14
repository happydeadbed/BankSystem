namespace BankApi.Models
{
    public class LoanPayment
    {
        public int Id { get; set; }

        public int LoanId { get; set; }

        public Loan? Loan { get; set; }

        public DateTime DueDate { get; set; }

        public decimal AmountDue { get; set; }

        public decimal AmountPaid { get; set; }

        public bool IsPaid { get; set; }
    }
}