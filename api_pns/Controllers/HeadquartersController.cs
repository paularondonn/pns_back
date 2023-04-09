using Microsoft.AspNetCore.Mvc;

namespace api_pns.Controllers
{
    public class HeadquartersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
