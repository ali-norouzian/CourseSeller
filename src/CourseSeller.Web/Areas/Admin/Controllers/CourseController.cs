using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            return View();
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

        [Route("[area]/Courses/GetSubGroups/{id}")]
        public async Task<IActionResult> GetSubGroups(int id)
        {
            var subGroups = await _courseService.GetSubGroupsForManageCourse(id);



            return new JsonResult(subGroups);
        }
    }
}
