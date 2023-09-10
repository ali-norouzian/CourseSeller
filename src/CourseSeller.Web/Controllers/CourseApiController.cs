using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Controllers
{
    [ApiController]
    public class CourseApiController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseApiController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("/API/Search")]
        public async Task<IActionResult> Search(string q)
        {
            try
            {
                var courses = await _courseService.SearchByTitle(q);

                return Ok(courses);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return BadRequest();
            }
        }
    }
}
