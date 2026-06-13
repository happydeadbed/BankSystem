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

        public void GeneratePaymentSchedule(Loan loan)
        {
            decimal monthlyAmount = loan.OriginalAmount / loan.TotalMonths;
            for (int i = 0; i < loan.TotalMonths; i++)
            {
                var payment = new LoanPayment
                {
                    LoanId = loan.Id,
                    DueDate = loan.StartDate.AddMonths(i + 1),
                    Amount = monthlyAmount,
                    Status = "Pending"
                };
                _context.LoanPayments.Add(payment);
            }
            _context.SaveChanges();
        }

        public async Task CheckAndMarkMissedPayments()
        {
            var pendingPayments = await _context.LoanPayments
                .Where(p => p.Status == "Pending" && p.DueDate < DateTime.Now)
                .ToListAsync();

            foreach (var payment in pendingPayments)
            {
                payment.Status = "Missed";
            }

            await _context.SaveChangesAsync();
            await CheckAndDefaultLoans();
        }

        public async Task CheckAndDefaultLoans()
        {
            var activeLoans = await _context.Loans
                .Where(l => l.Status == "Active")
                .Include(l => l.LoanPayments)
                .ToListAsync();

            foreach (var loan in activeLoans)
            {
                var payments = loan.LoanPayments
                    .OrderBy(p => p.DueDate)
                    .ToList();

                int consecutiveMissed = 0;
                int maxConsecutive = 0;

                foreach (var payment in payments)
                {
                    if (payment.Status == "Missed")
                    {
                        consecutiveMissed++;
                        if (consecutiveMissed > maxConsecutive)
                            maxConsecutive = consecutiveMissed;
                    }
                    else
                    {
                        consecutiveMissed = 0;
                    }
                }

                if (maxConsecutive >= 4)
                {
                    loan.Status = "Defaulted";
                    await CreateMarketplaceListing(loan);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateMarketplaceListing(Loan loan)
        {
            var alreadyListed = await _context.MarketplaceListings
                .AnyAsync(m => m.LoanId == loan.Id);

            if (alreadyListed) return;

            var paidAmount = await _context.LoanPayments
                .Where(p => p.LoanId == loan.Id && p.Status == "Paid")
                .SumAsync(p => p.Amount);

            var remainingBalance = loan.OriginalAmount - paidAmount;
            var listedPrice = remainingBalance + (loan.OriginalAmount * 0.20m);

            _context.MarketplaceListings.Add(new MarketplaceListing
            {
                LoanId = loan.Id,
                CollateralDescription = loan.CollateralDescription,
                CollateralValue = loan.CollateralValue,
                OriginalLoanAmount = loan.OriginalAmount,
                RemainingBalance = remainingBalance,
                ListedPrice = listedPrice,
                Status = "Listed"
            });

            await _context.SaveChangesAsync();
        }
    }
}