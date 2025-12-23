using Cortex.DTOs;
using Cortex.DTOs.auth;

namespace Cortex.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(string username, string password);
        Task<bool> CreateStaffIfNotExistsAsync(string username, string password, string firstName, string lastName, string role = "Admin");
        Task<bool> CreatePatientIfNotExistsAsync(string username, string password, string firstName, string lastName);
    }
}
