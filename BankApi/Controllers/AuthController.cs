using BankApi.Data;
using BankApi.DTOs;
using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly AuthService _authService;

        public AuthController(
            AppDbContext context,
            IConfiguration config,
            AuthService authService)
        {
            _context = context;
            _config = config;
            _authService = authService;
        }

        [HttpPost("admin-login")]
        public IActionResult AdminLogin(LoginDto dto)
        {
            var admin = _context.Admins
                .FirstOrDefault(a => a.Username == dto.Username);

            if (admin == null)
                return Unauthorized("Invalid credentials");

            bool valid = _authService.VerifyPassword(
                dto.Password,
                admin.PasswordHash);

            if (!valid)
                return Unauthorized("Invalid credentials");

            var token = GenerateToken(admin.Username, "Admin");

            return Ok(new { Token = token, Role = "Admin" });
        }

        [HttpPost("customer-login")]
        public IActionResult CustomerLogin(LoginDto dto)
        {
            var customer = _context.Customers
                .FirstOrDefault(c => c.CustomerNumber == dto.Username);

            if (customer == null)
                return Unauthorized("Invalid credentials");

            bool valid = _authService.VerifyPassword(
                dto.Password,
                customer.PasswordHash);

            if (!valid)
                return Unauthorized("Invalid credentials");

            var token = GenerateToken(
                customer.CustomerNumber,
                "Customer");

            return Ok(new
            {
                Token = token,
                Role = "Customer"
            });
        }

        [HttpPost("register-customer")]
        public IActionResult RegisterCustomer(Customer customer)
        {
            customer.CustomerNumber =
                $"CUS-{DateTime.Now.Ticks}";

            customer.PasswordHash =
                _authService.HashPassword(customer.PasswordHash);

            customer.CreatedDate = DateTime.UtcNow;

            _context.Customers.Add(customer);

            _context.SaveChanges();

            return Ok(customer);
        }

        private string GenerateToken(
            string username,
            string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["Jwt:Key"]!));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminDto dto)
        {
            if (_context.Admins.Any(x => x.Username == dto.Username))
                return BadRequest("Username already exists.");

            var admin = new Admin
            {
                Username = dto.Username,
                PasswordHash = _authService.HashPassword(dto.Password)
            };

            _context.Admins.Add(admin);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                admin.Id,
                admin.Username
            });
        }


    }
}