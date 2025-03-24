using Azure.Core;
using Google;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.Services;

[ApiController]
[Route("authenticate")]
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
    public async Task<IActionResult> AuthenticateUser([FromBody] LoginRequest request)
    {
        string? firebaseToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(firebaseToken))
            return Unauthorized(new { message = "No token provided" });

        // ✅ Verify Firebase Token & Extract Email
        string? firebaseEmail = await _firebaseService.VerifyFirebaseToken(firebaseToken);
        if (firebaseEmail == null)
            return Unauthorized(new { message = "Invalid Firebase token" });

        // ✅ Check if Customer exists in the database
        var customer = await _dbContext.Customers
            .Where(c => c.Email == request.Email)
            .FirstOrDefaultAsync();

        if (customer == null)
            return NotFound(new { message = "User not found" });

        // ✅ Generate JWT Token
        string jwtToken = _jwtTokenGenerator.GenerateJwtToken(customer);

        return Ok(new { token = jwtToken });
    }
}
