namespace BankApi.DTOs
{
    public class WithdrawDto
    {
        public string AccountNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
}