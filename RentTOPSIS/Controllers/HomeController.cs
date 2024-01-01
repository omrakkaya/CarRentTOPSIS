using Microsoft.AspNetCore.Mvc;

namespace RentTOPSIS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/Car");
        }
    }
}
