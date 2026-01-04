using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Staff
{
    public class DetailsModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;

        public DetailsModel(HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public Entities.Staff Staff { get; set; } = default!;

        // Display properties
        public string DepartmentName { get; set; } = "Unknown Department";

        // Statistics
        public int MonthlyAppointments { get; set; } = 0;
        public int TotalPatients { get; set; } = 0;
        public decimal? AverageRating { get; set; } = 0;
        public int LeaveBalance { get; set; } = 20;
        public int AttendancePercentage { get; set; } = 95;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load staff with department information
            Staff = await _context.Staff
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.StaffID == id);

            if (Staff == null)
            {
                return NotFound();
            }

            // Set department name
            if (Staff.Department != null)
            {
                DepartmentName = Staff.Department.DepartmentName;
            }

            // Load statistics
            await LoadStatisticsAsync(id.Value);

            return Page();
        }

        private async Task LoadStatisticsAsync(int staffId)
        {
            try
            {
                // Count monthly appointments (current month)
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                MonthlyAppointments = await _context.Appointments
                    .Where(a => a.DoctorId == staffId &&
                               a.AppointmentDate >= startDate &&
                               a.AppointmentDate <= endDate)
                    .CountAsync();

                // Count total patients for this staff member
                if (Staff.Position == "Doctor" || Staff.Position.Contains("Doctor"))
                {
                    TotalPatients = await _context.Staff
                        .CountAsync(p => p.StaffID == staffId);
                }

                // Calculate average rating (if you have a rating system)
                // This is a placeholder - you'll need to implement based on your data model
                AverageRating = 4.5m;

                // Calculate attendance percentage (placeholder)
                // You would typically have an attendance tracking system
                AttendancePercentage = 95;

                // Leave balance (placeholder)
                // You would typically have a leave management system
                LeaveBalance = 20;
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }
    }
}