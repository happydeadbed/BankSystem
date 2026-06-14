using BankApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> GetKpis()
        {
            var totalCustomers = await _context.Customers.CountAsync();

            var totalAccounts = await _context.BankAccounts.CountAsync();

            var totalTransactions = await _context.Transactions.CountAsync();

            var totalLoans = await _context.Loans.CountAsync();

            var activeLoans = await _context.Loans
                .CountAsync(l => l.Status == "Active");

            var defaultedLoans = await _context.Loans
                .CountAsync(l => l.Status == "Defaulted");

            var marketplaceListings = await _context.MarketplaceListings
                .CountAsync();

            var totalBalance = await _context.BankAccounts
                .SumAsync(a => (decimal?)a.Balance) ?? 0;

            return Ok(new
            {
                TotalCustomers = totalCustomers,
                TotalAccounts = totalAccounts,
                TotalTransactions = totalTransactions,
                TotalLoans = totalLoans,
                ActiveLoans = activeLoans,
                DefaultedLoans = defaultedLoans,
                MarketplaceListings = marketplaceListings,
                TotalBalance = totalBalance
            });
        }
    }
}