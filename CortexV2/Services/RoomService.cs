using static System.Net.WebRequestMethods;

namespace Cortex.Services
{
    public class RoomService
    {
        private readonly HttpClient _http;

        public RoomService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<int> GetRoomCountAsync()
        {
            return await _http.GetFromJsonAsync<int>("api/Rooms/count");
        }
    }
}
