using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseSeller.Web.Areas.UserPanel.Controllers;
using static CourseSeller.Core.Services.CourseService;

namespace CourseSeller.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IOrderService _orderService;

        public CourseController(ICourseService courseService, IOrderService orderService)
        {
            _courseService = courseService;
            _orderService = orderService;
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

        [Route("[controller]/{courseId}")]
        public async Task<IActionResult> ShowCourse(int courseId)
        {
            var course = await _courseService.GetCourseByForShowSingle(courseId);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [Authorize]
        [Route("[controller]/{courseId}/BuyCourse")]
        public async Task<IActionResult> BuyCourse(int courseId)
        {
            var orderId = await _orderService.CreateOrder(User.Identity.Name, courseId);

            return RedirectToAction("ShowOrder", "Order", new { area = "UserPanel", orderId = orderId });
        }
    }
}
