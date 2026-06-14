using BankApi.Data;
using BankApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerTransactions(int customerId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpPost("reverse/{id}")]
        public async Task<IActionResult> ReverseTransaction(int id)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return NotFound("Transaction not found");

            if (transaction.IsReversed)
                return BadRequest("Transaction already reversed");

            if (transaction.TransactionType == "Deposit")
            {
                var account = await _context.BankAccounts
                    .FindAsync(transaction.FromAccountId);

                if (account != null)
                    account.Balance -= transaction.Amount;
            }
            else if (transaction.TransactionType == "Withdraw")
            {
                var account = await _context.BankAccounts
                    .FindAsync(transaction.FromAccountId);

                if (account != null)
                    account.Balance += transaction.Amount;
            }
            else if (transaction.TransactionType == "Transfer")
            {
                var fromAccount = await _context.BankAccounts
                    .FindAsync(transaction.FromAccountId);

                var toAccount = await _context.BankAccounts
                    .FindAsync(transaction.ToAccountId);

                if (fromAccount != null)
                    fromAccount.Balance += transaction.Amount;

                if (toAccount != null)
                    toAccount.Balance -= transaction.Amount;
            }

            transaction.IsReversed = true;

            await _context.SaveChangesAsync();

            return Ok("Transaction reversed successfully");
        }
    }
}