using BankApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _context.Admins
                .Select(a => new
                {
                    a.Id,
                    a.Username
                })
                .ToListAsync();

            return Ok(admins);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
                return NotFound();

            _context.Admins.Remove(admin);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                admin.Id,
                admin.Username
            });
        }
    }
}