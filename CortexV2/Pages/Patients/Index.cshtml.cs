// Areas/Web/Pages/Patients/Index.cshtml.cs
using Cortex.DTOs.Users;
using Cortex.Entities;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cortex.UI.Areas.Web.Pages.Patients
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly PatientsService _patients;
        HospitalManagementSystemContext _context;

        public IndexModel(PatientsService patients,HospitalManagementSystemContext context)
        {
            _patients = patients;
            _context = context;
        }

        public List<PatientDto> Patients { get; set; } = new();

        public async Task OnGetAsync()
        {
            Patients = await _patients.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var ok = await _patients.DeleteAsync(id);
            if (ok) TempData["StatusMessage"] = "Patient deleted successfully.";
            else TempData["StatusMessage"] = "Delete failed.";

            // Redirect so OnGetAsync runs again
            return RedirectToPage();
        }
    }
}
