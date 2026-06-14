using BankApi.Data;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class LoanService
    {
        private readonly AppDbContext _context;

        public LoanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GeneratePaymentSchedule(Loan loan)
        {
            decimal monthlyPayment = loan.Amount / loan.DurationMonths;

            for (int i = 1; i <= loan.DurationMonths; i++)
            {
                _context.LoanPayments.Add(new LoanPayment
                {
                    LoanId = loan.Id,
                    DueDate = loan.StartDate.AddMonths(i),
                    AmountDue = monthlyPayment,
                    AmountPaid = 0,
                    IsPaid = false
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task CheckAndMarkMissedPayments()
        {
            var overduePayments = await _context.LoanPayments
                .Where(p => !p.IsPaid && p.DueDate < DateTime.UtcNow)
                .ToListAsync();

            await _context.SaveChangesAsync();

            await CheckAndDefaultLoans();
        }

        public async Task CheckAndDefaultLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.Payments)
                .Where(l => l.Status == "Active")
                .ToListAsync();

            foreach (var loan in loans)
            {
                int consecutiveMissed = 0;

                foreach (var payment in loan.Payments.OrderBy(p => p.DueDate))
                {
                    if (!payment.IsPaid &&
                        payment.DueDate < DateTime.UtcNow)
                    {
                        consecutiveMissed++;
                    }
                    else
                    {
                        consecutiveMissed = 0;
                    }

                    if (consecutiveMissed >= 4)
                    {
                        loan.Status = "Defaulted";

                        await CreateMarketplaceListing(loan);

                        break;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateMarketplaceListing(Loan loan)
        {
            bool exists = await _context.MarketplaceListings
                .AnyAsync(x => x.LoanId == loan.Id);

            if (exists)
                return;

            var listing = new MarketplaceListing
            {
                ListingNumber = $"LST-{Guid.NewGuid().ToString()[..8]}",
                LoanId = loan.Id,
                CustomerId = loan.CustomerId,
                ListingDate = DateTime.UtcNow,
                Status = "Listed"
            };

            _context.MarketplaceListings.Add(listing);

            await _context.SaveChangesAsync();
        }
    }
}