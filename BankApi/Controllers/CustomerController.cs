using BankApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Models.Customer model)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            customer.FullName = model.FullName;

            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return Ok("Customer deleted");
        }

        [HttpGet("summary/{id}")]
        public async Task<IActionResult> CustomerSummary(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.BankAccounts)
                .Include(c => c.Loans)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            return Ok(new
            {
                CustomerId = customer.Id,
                customer.CustomerNumber,
                customer.FullName,
                TotalBalance = customer.BankAccounts.Sum(a => a.Balance),
                AccountCount = customer.BankAccounts.Count,
                LoanCount = customer.Loans.Count
            });
        }
    }
}