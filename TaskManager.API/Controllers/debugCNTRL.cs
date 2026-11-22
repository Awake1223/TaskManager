using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Controllers
{
    public class debugCNTRL : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
