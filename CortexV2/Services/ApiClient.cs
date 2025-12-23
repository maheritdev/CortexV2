using Cortex.DTOs.auth;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;


namespace Cortex.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        public ApiClient(HttpClient http)
        {
            _http = http;
            /*// For development only — ignore SSL validation
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _http = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://cortex.runasp.net/") // match your API port
            };*/
        }


        public async Task<AuthResponseDto?> LoginAsync(string username, string password)
        {
            var payload = new { username = username, password = password };
            var res = await _http.PostAsJsonAsync("/api/Auth/login", payload);
            if (!res.IsSuccessStatusCode) return null;
            var dto = await res.Content.ReadFromJsonAsync<AuthResponseDto>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return dto;
        }


        public async Task<T?> GetAsync<T>(string path)
        {
            var res = await _http.GetAsync(path);
            if (!res.IsSuccessStatusCode) return default;
            return await res.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        public async Task<T?> PostAsync<T>(string path, object body)
        {
            var res = await _http.PostAsJsonAsync(path, body);
            if (!res.IsSuccessStatusCode) return default;
            return await res.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
