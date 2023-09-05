using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static CourseSeller.Core.Services.CourseService;

namespace CourseSeller.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index(int pageId = 1, int take = 9, string filter = null,
            string getType = TypeForAll, string orderByType = OrderByDate,
            int startPrice = 0, int endPrice = int.MaxValue, List<int> selectedGroups = null)
        {
            ViewData["pageId"] = pageId;

            var filteredCourses = await _courseService.GetAllGroups();
            ViewData["groups"] = filteredCourses;
            ViewData["selectedGroups"] = selectedGroups;

            return View(await _courseService.GetAllCourse(pageId, take, filter, getType, orderByType, startPrice, endPrice, selectedGroups));
        }
    }
}
