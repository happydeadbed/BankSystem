using BankApi.Data;
using BankApi.DTOs;
using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LoanService _loanService;

        public LoanController(
            AppDbContext context,
            LoanService loanService)
        {
            _context = context;
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loans = await _context.Loans
                .Include(l => l.Customer)
                .Include(l => l.Payments)
                .ToListAsync();

            return Ok(loans);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLoan(CreateLoanDto dto)
        {
            var loan = new Loan
            {
                LoanNumber = $"LN-{DateTime.Now.Ticks}",
                CustomerId = dto.CustomerId,
                Amount = dto.Amount,
                InterestRate = dto.InterestRate,
                DurationMonths = dto.DurationMonths,
                StartDate = DateTime.UtcNow,
                Status = "Active"
            };

            _context.Loans.Add(loan);

            await _context.SaveChangesAsync();

            await _loanService.GeneratePaymentSchedule(loan);

            return Ok(loan);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Payments)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound();

            return Ok(loan);
        }

        [HttpGet("{id}/payments")]
        public async Task<IActionResult> GetPayments(int id)
        {
            var payments = await _context.LoanPayments
                .Where(p => p.LoanId == id)
                .OrderBy(p => p.DueDate)
                .ToListAsync();

            return Ok(payments);
        }

        [HttpPost("payment/{paymentId}")]
        public async Task<IActionResult> PayLoan(int paymentId)
        {
            var payment = await _context.LoanPayments
                .FindAsync(paymentId);

            if (payment == null)
                return NotFound();

            payment.IsPaid = true;
            payment.AmountPaid = payment.AmountDue;

            await _context.SaveChangesAsync();

            return Ok("Payment successful");
        }

        [HttpPost("check-missed")]
        public async Task<IActionResult> CheckMissedPayments()
        {
            await _loanService.CheckAndMarkMissedPayments();

            return Ok("Missed payments checked");
        }
    }
}