﻿using CourseSeller.Core.Services.Interfaces;
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

        [Authorize]
        [Route("[controller]/{courseId}/ListComments/{pageId?}")]
        public async Task<IActionResult> ListComments(int courseId, int pageId = 1)
        {
            var comments = await _courseService.GetCourseComment(courseId, pageId);

            return View("ListComments", comments);
        }
    }
}
