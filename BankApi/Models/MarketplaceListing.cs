namespace BankApi.Models
{
    public class MarketplaceListing
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public Loan? Loan { get; set; }
        public string CollateralDescription { get; set; } = string.Empty;
        public decimal CollateralValue { get; set; }
        public decimal OriginalLoanAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal ListedPrice { get; set; }
        public string Status { get; set; } = "Listed";
        public string BuyerName { get; set; } = string.Empty;
        public DateTime? SoldAt { get; set; }
    }
}