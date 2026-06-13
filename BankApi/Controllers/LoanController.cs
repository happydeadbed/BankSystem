using BankApi.Data;
using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LoanService _loanService;

        public LoanController(AppDbContext context, LoanService loanService)
        {
            _context = context;
            _loanService = loanService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var loans = await _context.Loans
                    .Include(l => l.Customer)
                    .Include(l => l.Account)
                    .ToListAsync();
                return Ok(loans);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> IssueLoan(Loan model)
        {
            try
            {
                model.MonthlyAmount = model.OriginalAmount / model.TotalMonths;
                model.StartDate = DateTime.Now;
                model.EndDate = DateTime.Now.AddMonths(model.TotalMonths);
                model.Status = "Active";

                _context.Loans.Add(model);
                await _context.SaveChangesAsync();

                _loanService.GeneratePaymentSchedule(model);

                return Ok(model);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpGet("{id}/payments")]
        public async Task<IActionResult> GetPayments(int id)
        {
            try
            {
                var payments = await _context.LoanPayments
                    .Where(p => p.LoanId == id)
                    .OrderBy(p => p.DueDate)
                    .ToListAsync();
                return Ok(payments);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("payment/{id}/pay")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            try
            {
                var payment = await _context.LoanPayments.FindAsync(id);
                if (payment == null) return NotFound("Payment not found.");

                payment.Status = "Paid";
                await _context.SaveChangesAsync();
                return Ok("Payment marked as paid.");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("check-missed")]
        public async Task<IActionResult> CheckMissed()
        {
            try
            {
                await _loanService.CheckAndMarkMissedPayments();
                return Ok("Missed payments checked and updated.");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}