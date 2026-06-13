using BankApi.Data;
using BankApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var transactions = await _context.Transactions
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
                return Ok(transactions);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetByAccount(int accountId)
        {
            try
            {
                var transactions = await _context.Transactions
                    .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
                return Ok(transactions);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("{id}/reverse")]
        public async Task<IActionResult> Reverse(int id)
        {
            try
            {
                var original = await _context.Transactions.FindAsync(id);
                if (original == null) return NotFound("Transaction not found.");
                if (original.IsReversed) return BadRequest("Transaction already reversed.");

                original.IsReversed = true;

                if (original.TransactionType == "Deposit" && original.FromAccountId.HasValue)
                {
                    var acc = await _context.BankAccounts.FindAsync(original.FromAccountId.Value);
                    if (acc != null) acc.Balance -= original.Amount;
                }
                else if (original.TransactionType == "Withdrawal" && original.FromAccountId.HasValue)
                {
                    var acc = await _context.BankAccounts.FindAsync(original.FromAccountId.Value);
                    if (acc != null) acc.Balance += original.Amount;
                }
                else if (original.TransactionType == "Transfer")
                {
                    var from = await _context.BankAccounts.FindAsync(original.FromAccountId);
                    var to = await _context.BankAccounts.FindAsync(original.ToAccountId);
                    if (from != null) from.Balance += original.Amount;
                    if (to != null) to.Balance -= original.Amount;
                }

                _context.Transactions.Add(new Transaction
                {
                    TransactionType = "Reversal",
                    Amount = original.Amount,
                    Description = "Reversal of transaction #" + original.Id,
                    FromAccountId = original.FromAccountId,
                    ToAccountId = original.ToAccountId,
                    Date = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Ok("Transaction reversed.");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}