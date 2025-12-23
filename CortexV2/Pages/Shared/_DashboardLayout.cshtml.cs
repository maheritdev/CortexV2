using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cortex.pages.Shared
{
    [Authorize(Roles = "Stuff")]
    public class _DashboardLayoutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
