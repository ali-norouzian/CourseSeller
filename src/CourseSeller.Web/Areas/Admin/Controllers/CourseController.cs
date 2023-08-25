using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [Route("[area]/Courses")]
        public async Task<IActionResult> Index()
        {
            return View(await _courseService.GetAllCoursesForAdmin());
        }

        [Route("[area]/Courses/Create")]
        public async Task<IActionResult> CreateCourse()
        {
            var groups = await _courseService.GetGroupsForManageCourse();
            ViewData["Groups"] = groups;
            var subGroups = await _courseService.GetSubGroupsForManageCourse(int.Parse(groups.First().Value));
            ViewData["SubGroups"] = subGroups;
            var teachers = await _courseService.GetAllTeachers();
            ViewData["Teachers"] = teachers;
            var levels = await _courseService.GetAllLevels();
            ViewData["Levels"] = levels;
            var statuses = await _courseService.GetAllStatus();
            ViewData["Statuses"] = statuses;

            return View();
        }

        // Its for GET:CreateCourse
        [Route("[area]/Courses/GetSubGroups/{id}")]
        public async Task<IActionResult> GetSubGroups(int id)
        {
            var subGroups = await _courseService.GetSubGroupsForManageCourse(id);

            return new JsonResult(subGroups);
        }

        [HttpPost]
        [Route("[area]/Courses/Create")]
        public async Task<IActionResult> CreateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp)
        {
            //if (!ModelState.IsValid)
            //    return View(course);

            await _courseService.CreateCourse(course, imgCourseUp, demoUp);

            return View(course);
        }

    }
}
