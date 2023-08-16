using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}