using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize]
        [Route("[controller]/{courseId}/Download/{episodeId}")]
        public async Task<IActionResult> DownloadCourse(int courseId, int episodeId)
        {
            var episode = await _courseService.GetEpisodeById(episodeId);
            var fileName = episode.EpisodeFileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot/Courses/Episodes",
                fileName
            );
            if (episode.IsFree ||
                await _orderService.UserHasCourse(User.Identity.Name, episode.CourseId)
                )
            {
                // Download
                byte[] file = await System.IO.File.ReadAllBytesAsync(filePath);

                return File(file, "application/force-download", fileName);
            }

            return Forbid();
        }

        [Authorize]
        [HttpPost]
        [Route("[controller]/{courseId}/CreateComment")]
        public async Task<IActionResult> CreateComment(Comment comment, int courseId)
        {
            comment.IsDelete = false;
            comment.CreateDateTime = DateTime.Now;
            comment.UserId = await _orderService.GetUserIdByUserName(User.Identity.Name);

            await _courseService.CreateComment(comment);

            var comments = await _courseService.GetCourseComment(courseId);

            return View("ListComments", comments);
        }

        // SSR Comment list with jquery
        [Authorize]
        [Route("[controller]/{courseId}/ListComments/{pageId?}")]
        public async Task<IActionResult> ListComments(int courseId, int pageId = 1)
        {
            var comments = await _courseService.GetCourseComment(courseId, pageId);

            return View("ListComments", comments);
        }

        [Authorize]
        [Route("[controller]/{courseId}/Vote")]
        public async Task<IActionResult> Vote(int courseId)
        {
            if (!await _courseService.IsFree(courseId) &&
                !await _orderService.UserHasCourse(User.Identity.Name, courseId))
                ViewData["AccessDeny"] = true;

            return PartialView(await _courseService.GetCourseVote(courseId));
        }

        [Authorize]
        [Route("[controller]/{courseId}/AddVote")]
        public async Task<IActionResult> CreateVote(int courseId, [FromQuery] bool vote)
        {
            await _courseService.CreateVote(
                await _orderService.GetUserIdByUserName(User.Identity.Name),
                courseId, vote);

            return PartialView("Vote", await _courseService.GetCourseVote(courseId));
        }

        [Route("[controller]/{courseId}/EpisodeVideo/{episodeId}")]
        public async Task<IActionResult> EpisodeVideo(int courseId, int episodeId)
        {
            var episodeFileName = (await _courseService.GetEpisodeById(episodeId)).EpisodeFileName;

            if (await _courseService.IsFree(courseId) ||
                await _courseService.EpisodeIsFree(episodeId))
                return View(model: episodeFileName);
            if (User.Identity.IsAuthenticated)
            {
                if (await _orderService.UserHasCourse(User.Identity.Name, courseId))
                {
                    return View(model: episodeFileName);
                }
            }

            return Forbid();
        }
    }
}
