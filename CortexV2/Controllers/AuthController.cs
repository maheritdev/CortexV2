using Cortex.DTOs;
using Cortex.DTOs.auth;
using Cortex.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { error = "Username and password are required." });

            var authResponse = await _auth.LoginAsync(dto.Username, dto.Password);
            if (authResponse == null) return Unauthorized(new { error = "Invalid credentials." });

            return Ok(authResponse);
        }

      /*  public async Task<IActionResult> AccessDenied(string url)
        {
            return View();
        }*/

        // Admin-only seeding endpoints (optional) - recommend removing in production
        [HttpPost("seed/admin")]
        public async Task<IActionResult> SeedAdmin()
        {
            var ok = await _auth.CreateStaffIfNotExistsAsync("admin", "AdminPassword123!", "Admin", "User", "Admin");
            if (!ok) return BadRequest(new { error = "Admin already exists or creation failed." });
            return Ok(new { message = "Admin seeded (username=admin, password=AdminPassword123!). Change password immediately." });
        }

        [HttpPost("seed/patient")]
        public async Task<IActionResult> SeedPatient()
        {
            var ok = await _auth.CreatePatientIfNotExistsAsync("patient1", "PatientPassword123!", "John", "Doe");
            if (!ok) return BadRequest(new { error = "Patient already exists or creation failed." });
            return Ok(new { message = "Patient seeded (username=patient1, password=PatientPassword123!)." });
        }
    }
}
