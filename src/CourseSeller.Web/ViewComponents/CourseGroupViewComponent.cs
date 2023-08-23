using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.ViewComponents;

public class CourseGroupViewComponent : ViewComponent
{
    private readonly ICourseService _courseService;

    public CourseGroupViewComponent(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cg = await _courseService.GetAll();

        return await Task.FromResult((IViewComponentResult)View("CourseGroup", cg));
    }
}

