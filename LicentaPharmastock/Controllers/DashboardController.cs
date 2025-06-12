namespace LicentaPharmastock.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace LicentaPharmastock.Controllers
    {
        [Authorize]
        public class DashboardController : Controller
        {
            public IActionResult RoleSelection()
            {
                return View();
            }
        }
    }

}
