using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Services;

[ApiController]
[Route("authenticatication")]
public class AuthController : ControllerBase
{
    private readonly FirebaseService _firebaseService;
    private readonly CoffeeShop01Context _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthController(FirebaseService firebaseService, CoffeeShop01Context dbContext, IJwtTokenGenerator jwtTokenGenerator)
    {
        _firebaseService = firebaseService;
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpPost]
    public async Task<IActionResult> AuthenticateUser()
    {
        string? firebaseToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(firebaseToken))
            return Unauthorized(new { message = "No token provided" });

        // ✅ Verify Firebase Token & Extract Email
        string? firebaseEmail = await _firebaseService.VerifyFirebaseToken(firebaseToken);
        if (firebaseEmail == null)
            return Unauthorized(new { message = "Invalid Firebase token" });

        // ✅ Check if Customer exists
        var customer = await _dbContext.Customers
            .Include(c => c.Wallets) // Ensure we fetch Wallets too
            .FirstOrDefaultAsync(c => c.Email == firebaseEmail);

        if (customer == null)
        {
            // ✅ Auto-Register New Customer
            customer = new Customer
            {
                FirstName = "New", // Can be updated later
                LastName = "User",
                Email = firebaseEmail,
                PhoneNumber = null,
                Address = null,
                Description = "Registered via Google login",
                Status = 1, // Assuming 1 means active
                Wallets = new List<Wallet>() // Initialize wallet list
            };

            // ✅ Create an empty wallet for the new customer
            var newWallet = new Wallet
            {
                Balance = 0.0M,
                CreateDate = DateTime.UtcNow,
                Status = 1, // Assuming 1 means active
                Customer = customer
            };

            customer.Wallets.Add(newWallet);

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
        }

        // ✅ Generate JWT Token
        string jwtToken = _jwtTokenGenerator.GenerateJwtToken(customer);

        return Ok(new { token = jwtToken, user = customer });
    }
}
