using Microsoft.EntityFrameworkCore;

namespace BankApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Customer> Customers { get; set; }
        public DbSet<Models.BankAccount> BankAccounts { get; set; }
        public DbSet<Models.Transaction> Transactions { get; set; }
        public DbSet<Models.Loan> Loans { get; set; }
        public DbSet<Models.LoanPayment> LoanPayments { get; set; }
        public DbSet<Models.MarketplaceListing> MarketplaceListings { get; set; }
        public DbSet<Models.Admin> Admins { get; set; }




    }
}
