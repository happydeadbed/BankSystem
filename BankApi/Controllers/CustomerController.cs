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
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await _context.Customers.ToListAsync();
                return Ok(customers);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Customer model)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null) return NotFound("Customer not found.");

                customer.FullName = model.FullName;
                customer.Username = model.Username;
                customer.Password = model.Password;

                await _context.SaveChangesAsync();
                return Ok(customer);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null) return NotFound("Customer not found.");

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return Ok("Customer deleted successfully.");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
