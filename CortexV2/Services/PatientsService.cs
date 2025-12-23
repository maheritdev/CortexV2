using Cortex.DTOs.Users;

namespace Cortex.Services
{
    public class PatientsService
    {
        private readonly HttpClient _http;

        public PatientsService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<PatientDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<PatientDto>>("api/patients") ?? new List<PatientDto>();
        }

        public async Task<PatientDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<PatientDto>($"api/patients/{id}");
        }

        public async Task<PatientDto?> CreateAsync(PatientDto input)
        {
            var res = await _http.PostAsJsonAsync("api/patients", input);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<PatientDto>();
        }

        public async Task<bool> UpdateAsync(int id, PatientDto input)
        {
            var res = await _http.PutAsJsonAsync($"api/patients/{id}", input);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/patients/{id}");
            return res.IsSuccessStatusCode;
        }

        public async Task<int> GetPatientsCountAsync()
        {
            return await _http.GetFromJsonAsync<int>("api/patients/count");
        }
    }
}
