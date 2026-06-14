namespace BankApi.DTOs
{
    public class CreateLoanDto
    {
        public int CustomerId { get; set; }

        public decimal Amount { get; set; }

        public decimal InterestRate { get; set; }

        public int DurationMonths { get; set; }
    }
}