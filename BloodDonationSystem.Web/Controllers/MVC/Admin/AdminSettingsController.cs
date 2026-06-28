using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC.Admin
{
    [Authorize(Roles = "Admin")]

    public class AdminSettingsController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Settings/Index.cshtml");
        }
    }
}