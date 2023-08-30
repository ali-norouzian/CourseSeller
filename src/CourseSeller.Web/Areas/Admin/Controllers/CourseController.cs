#nullable enable
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Entities.Courses;
using CourseSeller.DataLayer.Migrations;
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

        private async Task SetViewModelsForCreateUpdate(Course? course = null)
        {
            var groups = await _courseService.GetGroupsForManageCourse(course?.GroupId);
            ViewData["Groups"] = groups;
            var subGroups = await _courseService.GetSubGroupsForManageCourse(int.Parse(groups.First().Value), course?.SubGroupId);
            ViewData["SubGroups"] = subGroups;
            var teachers = await _courseService.GetAllTeachers(course?.TeacherId);
            ViewData["Teachers"] = teachers;
            var levels = await _courseService.GetAllLevels(course?.LevelId);
            ViewData["Levels"] = levels;
            var statuses = await _courseService.GetAllStatus(course?.StatusId);
            ViewData["Statuses"] = statuses;
        }

        [Route("[area]/Courses/Create")]
        public async Task<IActionResult> CreateCourse()
        {
            await SetViewModelsForCreateUpdate();

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
        [RequestSizeLimit(1 * 1024 * 1024 * 1024)] // 1 GB
        public async Task<IActionResult> CreateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp)
        {
            if (!ModelState.IsValid)
                return View(course);

            await _courseService.CreateCourse(course, imgCourseUp, demoUp);

            return View(course);
        }

        // Action for uploading descriptions images
        [HttpPost]
        [Route("/file-upload")]
        public async Task<IActionResult> UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/CourseDescriptionsImages",
                fileName);

            await using (var stream = new FileStream(path, FileMode.Create))
                await upload.CopyToAsync(stream);

            var url = $"{"/CourseDescriptionsImages/"}{fileName}";


            return Json(new { uploaded = true, url });
        }

        [Route("[area]/Courses/Update/{id}")]
        public async Task<IActionResult> UpdateCourse(int id)
        {
            var course = await _courseService.GetCourseById(id);

            await SetViewModelsForCreateUpdate(course);


            return View(course);
        }

        [HttpPost]
        [Route("[area]/Courses/Update/{id}")]
        public async Task<IActionResult> UpdateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp)
        {
            if (!ModelState.IsValid)
                return View(course);

            // ToDo: Check he want to access to files that is for him. not other people images.

            await _courseService.UpdateCourse(course, imgCourseUp, demoUp);


            return View();
        }

        [Route("[area]/Courses/{id}/Episodes")]
        public async Task<IActionResult> IndexEpisode(int id)
        {
            ViewData["CourseId"] = id;

            return View(await _courseService.ListCourseEpisodes(id));
        }

        [Route("[area]/Courses/{id}/Episodes/Create")]
        public async Task<IActionResult> CreateEpisode(int id)
        {
            return View();
        }

        [HttpPost]
        [Route("[area]/Courses/{id}/Episodes/Create")]
        public async Task<IActionResult> CreateEpisode(int id, CourseEpisode episode, IFormFile episodeFile)
        {
            episode.CourseId = id;
            if (!ModelState.IsValid || episodeFile == null)
                return View(episode);

            if (await _courseService.CheckExistFile(episodeFile.FileName))
            {
                ViewData["IsExistFile"] = true;

                return View(episode);
            }

            // ToDo: Check he want to access to files that is for him. not other people images.

            await _courseService.CreateEpisode(episode, episodeFile);

            return RedirectToAction(nameof(IndexEpisode), new { id = episode.CourseId });
        }

        [Route("[area]/Courses/{courseId}/Episodes/Update/{episodeId}")]
        public async Task<IActionResult> UpdateEpisode(int episodeId)
        {
            return View(await _courseService.GetEpisodeById(episodeId));
        }

        [HttpPost]
        [Route("[area]/Courses/{courseId}/Episodes/Update/{episodeId}")]
        public async Task<IActionResult> UpdateEpisode(int episodeId, CourseEpisode episode, IFormFile? episodeFile)
        {
            if (!ModelState.IsValid)
                return View(episode);

            if (episodeFile != null)
            {
                if (await _courseService.CheckExistFile(episodeFile.FileName))
                {
                    ViewData["IsExistFile"] = true;

                    return View(episode);
                }
            }

            // ToDo: Check he want to access to files that is for him. not other people images.

            await _courseService.UpdateEpisode(episode, episodeFile);

            return RedirectToAction(nameof(IndexEpisode), new { id = episode.CourseId });
        }
    }
}
