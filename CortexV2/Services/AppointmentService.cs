using Cortex.DTOs.Users;
using Cortex.Entities;
using Cortex.Viewmodel;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cortex.Services
{
    public class AppointmentService
    {
        private readonly HttpClient _http;
        private readonly HospitalManagementSystemContext _context;
        public AppointmentService(HospitalManagementSystemContext context, HttpClient http)
        {
            _http = http;
            _context = context;
        }

        public async Task<List<AppointmentDto>> GetAppointmentsAsync()
        {
            return await _http.GetFromJsonAsync<List<AppointmentDto>>("/api/Appointments") ?? new List<AppointmentDto>();
        }

        public async Task<bool> IsConflictAsync(int doctorId, DateTime start, DateTime end)
        {
            return await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId &&
                               a.StartTime < end &&
                               a.EndTime > start &&
                               a.Status == "Scheduled");
        }

        public async Task<Appointment> CreateAppointment(Appointment appointment)
        {
            if (await IsConflictAsync(appointment.DoctorId, appointment.StartTime, appointment.EndTime))
                throw new Exception("Conflict detected: Doctor already has an appointment at this time.");

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<int> GetAppointmentCountAsync()
        {
            return await _http.GetFromJsonAsync<int>("api/Appointments/count");
        }

        public async Task<List<RecentAppointmentDto>> GetRecentAppointmentsAsync()
        {
            var response = await _http.GetAsync("api/appointments/recent");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                //throw new Exception($"Failed to fetch recent appointments. Status: {response.StatusCode}, Error: {error}");
            }

            return await response.Content.ReadFromJsonAsync<List<RecentAppointmentDto>>();
        }
    }
}
