namespace Cortex.Viewmodel
{
    public class DashboardViewModel
    {
        // Top cards
        public int TotalPatients { get; set; }
        public int TodaysAppointments { get; set; }
        public int AvailableRooms { get; set; }
        public int PendingBills { get; set; }

        // Recent Appointments
        public List<RecentAppointmentDto> RecentAppointments { get; set; } = new();

        // System Status
        public string ServerUptime { get; set; }
        public double StorageUsage { get; set; } // %
        public double MemoryUsage { get; set; }  // %
    }

    public class RecentAppointmentDto
    {
        public int AppointmentId { get; set; }
        public string Patient { get; set; }
        public string Doctor { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }
    }
}
