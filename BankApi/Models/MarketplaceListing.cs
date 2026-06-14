namespace BankApi.Models
{
    public class MarketplaceListing
    {
        public int Id { get; set; }

        public string ListingNumber { get; set; } = string.Empty;

        public int LoanId { get; set; }

        public int CustomerId { get; set; }

        public DateTime ListingDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Listed";
    }
}