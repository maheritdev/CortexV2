using Cortex.DTOs.auth;

namespace Cortex.Services
{
    public interface IApiClient
    {
        Task<AuthResponseDto?> LoginAsync(string username, string password);
        Task<T?> GetAsync<T>(string path);
        Task<T?> PostAsync<T>(string path, object body);
    }
}
