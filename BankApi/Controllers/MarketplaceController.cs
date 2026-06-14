using BankApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MarketplaceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MarketplaceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var listings = await _context.MarketplaceListings
                .ToListAsync();

            return Ok(listings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var listing = await _context.MarketplaceListings
                .FirstOrDefaultAsync(x => x.Id == id);

            if (listing == null)
                return NotFound();

            return Ok(listing);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Models.MarketplaceListing listing)
        {
            listing.ListingNumber = $"LST-{DateTime.Now.Ticks}";
            listing.ListingDate = DateTime.UtcNow;

            _context.MarketplaceListings.Add(listing);

            await _context.SaveChangesAsync();

            return Ok(listing);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Models.MarketplaceListing model)
        {
            var listing = await _context.MarketplaceListings.FindAsync(id);

            if (listing == null)
                return NotFound();

            listing.Status = model.Status;

            await _context.SaveChangesAsync();

            return Ok(listing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var listing = await _context.MarketplaceListings.FindAsync(id);

            if (listing == null)
                return NotFound();

            _context.MarketplaceListings.Remove(listing);

            await _context.SaveChangesAsync();

            return Ok("Listing deleted");
        }
    }
}