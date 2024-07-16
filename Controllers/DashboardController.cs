using Microsoft.AspNetCore.Mvc;
using MESDashboard.Models;
using System.Collections.Generic;
using System.Linq;

namespace MESDashboard.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
