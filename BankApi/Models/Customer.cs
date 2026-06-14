namespace BankApi.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string CustomerNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}