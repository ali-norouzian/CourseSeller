using CourseSeller.Core.Security;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.Admin.Controllers
{

    [Authorize]
    [Area("Admin")]
    [PermissionChecker(PermissionCheckerAttribute.CourseGroupManagement)]
    public class CourseGroupController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseGroupController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _courseService.GetAllGroups());
        }

        [Route("[area]/[controller]/Create")]
        public async Task<IActionResult> CreateCourseGroup(int? parentId, int? groupId)
        {
            // Map it int hiddenform asp-for="ParentId"
            ViewData["ParentId"] = parentId;
            // This is for when we want to update
            if (groupId != null)
            {
                var group = await _courseService.GetGroup(groupId);
                ViewData["GroupId"] = group.GroupId;
                ViewData["GroupTitle"] = group.GroupTitle;
            }


            return View();
        }

        [HttpPost]
        [Route("[area]/[controller]/Create")]
        public async Task<IActionResult> CreateCourseGroup(CourseGroup group)
        {
            if (!ModelState.IsValid)
                return View(group);

            // This is for Update
            if (group.GroupId != null)
                await _courseService.UpdateGroup(group);
            else
                await _courseService.AddGroup(group);


            return RedirectToAction(nameof(Index));
        }
    }
}
