using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;

        public HomeController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _courseService.GetAllCourse());
        }
    }
}