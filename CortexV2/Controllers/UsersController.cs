using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public UsersController(HospitalManagementSystemContext context) => _context = context;

        // Helper: hash password
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Create User
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // Get All Users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // Get by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // Login (returns full profile)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var hashed = HashPassword(login.PasswordHash);

            var user = await _context.Users
                .Include(u => u.PatientProfile)
                .Include(u => u.StaffProfile)
                .FirstOrDefaultAsync(u => u.Username == login.Username && u.PasswordHash == hashed);

            if (user == null) return Unauthorized("Invalid credentials");

            return Ok(new
            {
                user.UserId,
                user.Username,
                user.Role,
                user.FullName,
                user.Email,
                user.Phone,
                PatientProfile = user.PatientProfile,
                StaffProfile = user.StaffProfile
            });
        }
    }
}
