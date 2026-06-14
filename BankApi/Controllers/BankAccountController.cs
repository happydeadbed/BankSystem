using BankApi.Data;
using BankApi.DTOs;
using BankApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankAccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BankAccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _context.BankAccounts
                .Include(a => a.Customer)
                .ToListAsync();

            return Ok(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BankAccount account)
        {
            account.AccountNumber = $"ACC-{DateTime.Now.Ticks}";
            account.CreatedDate = DateTime.UtcNow;
            account.Balance = 0;

            _context.BankAccounts.Add(account);

            await _context.SaveChangesAsync();

            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BankAccount model)
        {
            var account = await _context.BankAccounts.FindAsync(id);

            if (account == null)
                return NotFound();

            account.AccountType = model.AccountType;

            await _context.SaveChangesAsync();

            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.BankAccounts.FindAsync(id);

            if (account == null)
                return NotFound();

            _context.BankAccounts.Remove(account);

            await _context.SaveChangesAsync();

            return Ok("Account deleted");
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositDto dto)
        {
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == dto.AccountNumber);

            if (account == null)
                return NotFound("Account not found");

            account.Balance += dto.Amount;

            _context.Transactions.Add(new Transaction
            {
                TransactionNumber = $"TRN-{DateTime.Now.Ticks}",
                CustomerId = account.CustomerId,
                FromAccountId = account.Id,
                Amount = dto.Amount,
                TransactionType = "Deposit",
                TransactionDate = DateTime.UtcNow,
                IsReversed = false
            });

            await _context.SaveChangesAsync();

            return Ok(account);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawDto dto)
        {
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == dto.AccountNumber);

            if (account == null)
                return NotFound("Account not found");

            if (account.Balance < dto.Amount)
                return BadRequest("Insufficient funds");

            account.Balance -= dto.Amount;

            _context.Transactions.Add(new Transaction
            {
                TransactionNumber = $"TRN-{DateTime.Now.Ticks}",
                CustomerId = account.CustomerId,
                FromAccountId = account.Id,
                Amount = dto.Amount,
                TransactionType = "Withdraw",
                TransactionDate = DateTime.UtcNow,
                IsReversed = false
            });

            await _context.SaveChangesAsync();

            return Ok(account);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferDto dto)
        {
            var fromAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == dto.FromAccountNumber);

            var toAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == dto.ToAccountNumber);

            if (fromAccount == null || toAccount == null)
                return BadRequest("Invalid account number");

            if (fromAccount.Balance < dto.Amount)
                return BadRequest("Insufficient funds");

            fromAccount.Balance -= dto.Amount;
            toAccount.Balance += dto.Amount;

            _context.Transactions.Add(new Transaction
            {
                TransactionNumber = $"TRN-{DateTime.Now.Ticks}",
                CustomerId = fromAccount.CustomerId,
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,
                Amount = dto.Amount,
                TransactionType = "Transfer",
                TransactionDate = DateTime.UtcNow,
                IsReversed = false
            });

            await _context.SaveChangesAsync();

            return Ok("Transfer successful");
        }
    }
}