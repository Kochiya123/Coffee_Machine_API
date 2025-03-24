using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateJwtToken(Customer customer);
    }
}
