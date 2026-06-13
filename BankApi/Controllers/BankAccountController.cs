using BankApi.Data;
using BankApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BankAccountController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var accounts = await _context.BankAccounts
                    .Include(a => a.Customer)
                    .ToListAsync();
                return Ok(accounts);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(BankAccount account)
        {
            try
            {
                account.AccountNumber = "ACC" + DateTime.Now.Ticks.ToString().Substring(10, 9);
                account.Balance = 0;
                account.Status = "Active";

                _context.BankAccounts.Add(account);
                await _context.SaveChangesAsync();
                return Ok(account);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BankAccount model)
        {
            try
            {
                var account = await _context.BankAccounts.FindAsync(id);
                if (account == null) return NotFound("Account not found.");

                account.AccountType = model.AccountType;
                account.Status = model.Status;

                await _context.SaveChangesAsync();
                return Ok(account);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var account = await _context.BankAccounts.FindAsync(id);
                if (account == null) return NotFound("Account not found.");
                if (account.Balance > 0)
                    return BadRequest("Cannot delete account with remaining balance.");

                _context.BankAccounts.Remove(account);
                await _context.SaveChangesAsync();
                return Ok("Account deleted successfully.");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(int id, Transaction model)
        {
            try
            {
                var account = await _context.BankAccounts.FindAsync(id);
                if (account == null) return NotFound("Account not found.");
                if (model.Amount <= 0) return BadRequest("Amount must be greater than zero.");

                account.Balance += model.Amount;

                var transaction = new Transaction
                {
                    TransactionType = "Deposit",
                    Amount = model.Amount,
                    Description = model.Description,
                    FromAccountId = account.Id,
                    Date = DateTime.Now
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Deposit successful.", balance = account.Balance });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(int id, Transaction model)
        {
            try
            {
                var account = await _context.BankAccounts.FindAsync(id);
                if (account == null) return NotFound("Account not found.");
                if (model.Amount <= 0) return BadRequest("Amount must be greater than zero.");
                if (account.Balance < model.Amount) return BadRequest("Insufficient balance.");

                account.Balance -= model.Amount;

                var transaction = new Transaction
                {
                    TransactionType = "Withdrawal",
                    Amount = model.Amount,
                    Description = model.Description,
                    FromAccountId = account.Id,
                    Date = DateTime.Now
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Withdrawal successful.", balance = account.Balance });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(Transaction model)
        {
            try
            {
                var from = await _context.BankAccounts.FindAsync(model.FromAccountId);
                var to = await _context.BankAccounts.FindAsync(model.ToAccountId);

                if (from == null) return NotFound("Source account not found.");
                if (to == null) return NotFound("Destination account not found.");
                if (model.Amount <= 0) return BadRequest("Amount must be greater than zero.");
                if (from.Balance < model.Amount) return BadRequest("Insufficient balance.");

                from.Balance -= model.Amount;
                to.Balance += model.Amount;

                var transaction = new Transaction
                {
                    TransactionType = "Transfer",
                    Amount = model.Amount,
                    Description = model.Description,
                    FromAccountId = from.Id,
                    ToAccountId = to.Id,
                    Date = DateTime.Now
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Transfer successful.", balance = from.Balance });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}