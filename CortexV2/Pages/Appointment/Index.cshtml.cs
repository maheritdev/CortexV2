using Cortex.DTOs.Users;
using Cortex.Entities;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cortex.Pages.Appointment
{
    public class IndexModel : PageModel
    {
        private readonly AppointmentService _appointment;
        HospitalManagementSystemContext _context;

        public IndexModel(AppointmentService patients, HospitalManagementSystemContext context)
        {
            _appointment = patients;
            _context = context;
        }

        public List<AppointmentDto> Appointment { get; set; } = new();

        public async Task OnGetAsync()
        {
            Appointment = await _appointment.GetAppointmentsAsync();
        }

    }
}
