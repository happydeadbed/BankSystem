using Microsoft.EntityFrameworkCore.Metadata;
using System.Globalization;

namespace BankApi.Models
{
    public class Customer
    {

        public int Id { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;    
        public DateTime CreateAT {  get; set; } = DateTime.Now;

    }
}
