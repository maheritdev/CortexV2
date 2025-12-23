using Cortex.DTOs;
using Cortex.DTOs.auth;
using Cortex.DTOs.Users;
using Cortex.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cortex.Services
{
    public class AuthService : IAuthService
    {
        private readonly HospitalManagementSystemContext _db;
        private readonly JwtSettings _jwt;

        public AuthService(HospitalManagementSystemContext db, IOptions<JwtSettings> jwtOptions)
        {
            _db = db;
            _jwt = jwtOptions.Value;
        }

        public async Task<AuthResponseDto?> LoginAsync(string username, string password)
        {
            // Try staff first
            var staff = await _db.Staff.FirstOrDefaultAsync(s => s.Username == username);
            if (staff != null)
            {
                if (string.IsNullOrEmpty(staff.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, staff.PasswordHash))
                    return null;

                var token = GenerateTokenForStaff(staff);
                return new AuthResponseDto
                {
                    Token = token.token,
                    ExpiresAt = token.expiresAt,
                    Staff = MapToDto(staff)
                };
            }

            // Try patient
            var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Username == username);
            if (patient != null)
            {
                if (string.IsNullOrEmpty(patient.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, patient.PasswordHash))
                    return null;

                var token = GenerateTokenForPatient(patient);
                return new AuthResponseDto
                {
                    Token = token.token,
                    ExpiresAt = token.expiresAt,
                    Patient = MapToDto(patient)
                };
            }

            return null;
        }

        private StaffDto MapToDto(Staff s) => new StaffDto
        {
            StaffID = s.StaffID,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            ContactNumber = s.ContactNumber,
            DepartmentID = s.DepartmentId,
            Position = s.Position,
            Specialization = s.Specialization,
            Role = s.Role
        };

        private PatientDto MapToDto(Patient p) => new PatientDto
        {
            PatientID = p.PatientId,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Email = p.Email,
            ContactNumber = p.ContactNumber,
            Address = p.Address,
            //DateOfBirth = p.DateOfBirth == null ? DateTime.Parse(p.DateOfBirth.ToString() ) : DateTime.Now,
            MedicalHistory = p.MedicalHistory
        };

        private (string token, DateTime expiresAt) GenerateTokenForStaff(Staff s)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, s.StaffID.ToString()),
                new Claim(ClaimTypes.Name, $"{s.FirstName} {s.LastName}"),
                new Claim("username", s.Username ?? ""),
                new Claim("role", s.Role ?? s.Position ?? "Staff"),
                new Claim("usertype", "staff")
            };

            var (t, e) = GenerateJwtToken(claims);
            return (t, e);
        }

        private (string token, DateTime expiresAt) GenerateTokenForPatient(Patient p)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, p.PatientId.ToString()),
                new Claim(ClaimTypes.Name, $"{p.FirstName} {p.LastName}"),
                new Claim("username", p.Username ?? ""),
                new Claim("usertype", "patient")
            };

            var (t, e) = GenerateJwtToken(claims);
            return (t, e);
        }

        private (string token, DateTime expiresAt) GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenStr, expires);
        }

        public async Task<bool> CreateStaffIfNotExistsAsync(string username, string password, string firstName, string lastName, string role = "Admin")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return false;
            if (await _db.Staff.AnyAsync(s => s.Username == username)) return false;

            var staff = new Staff
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Position = role,
                Role = role,
                ContactNumber = "000"
            };
            _db.Staff.Add(staff);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreatePatientIfNotExistsAsync(string username, string password, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return false;
            if (await _db.Patients.AnyAsync(p => p.Username == username)) return false;

            var patient = new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                ContactNumber = "000",
                Address = "Unknown",
                RegistrationDate = DateTime.UtcNow,
                Status = "Active"
            };
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
