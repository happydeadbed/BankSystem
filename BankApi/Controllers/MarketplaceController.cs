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
    public class MarketplaceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MarketplaceController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var listings = await _context.MarketplaceListings
                    .Include(m => m.Loan)
                    .ToListAsync();
                return Ok(listings);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("{id}/sell")]
        public async Task<IActionResult> Sell(int id, MarketplaceListing model)
        {
            try
            {
                var listing = await _context.MarketplaceListings.FindAsync(id);
                if (listing == null) return NotFound("Listing not found.");
                if (listing.Status == "Sold") return BadRequest("Already sold.");

                listing.Status = "Sold";
                listing.BuyerName = model.BuyerName;
                listing.SoldAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok("Listing sold successfully.");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}