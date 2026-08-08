using MedicalSupply.Api.Contracts.Auth;
using MedicalSupply.Application.Abstractions.Security;
using MedicalSupply.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSupply.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenGenerator _tokenGenerator;

        public AuthController(ITokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = HardcodedUserStore.Users.FirstOrDefault(u =>
                u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == request.Password);

            if (user is null)
                return Unauthorized(new { code = "INVALID_CREDENTIALS", message = "Invalid email or password." });

            var token = _tokenGenerator.GenerateToken(
                userId: user.Email,
                email: user.Email,
                roles: new List<string> { user.Role });

            return Ok(new { token, email = user.Email, role = user.Role });
        }
    }
}