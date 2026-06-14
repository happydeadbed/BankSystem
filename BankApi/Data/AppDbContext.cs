using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins => Set<Admin>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();

        public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<Loan> Loans => Set<Loan>();

        public DbSet<LoanPayment> LoanPayments => Set<LoanPayment>();

        public DbSet<MarketplaceListing> MarketplaceListings => Set<MarketplaceListing>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.BankAccounts)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Loans)
                .WithOne(l => l.Customer)
                .HasForeignKey(l => l.CustomerId);

            modelBuilder.Entity<Loan>()
                .HasMany(l => l.Payments)
                .WithOne(p => p.Loan)
                .HasForeignKey(p => p.LoanId);
        }
    }
}