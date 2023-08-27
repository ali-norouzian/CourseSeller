using CourseSeller.Core.Convertors;
using CourseSeller.Core.DTOs.Course;
using CourseSeller.Core.Generators;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class CourseService : ICourseService
{
    public const byte TeacherRoleId = 2;

    private readonly MssqlContext _context;
    private readonly IImageUtils _imageUtils;

    public CourseService(MssqlContext context, IImageUtils imageUtils)
    {
        _context = context;
        _imageUtils = imageUtils;
    }

    public async Task<List<CourseGroup>> GetAll()
    {
        return await _context.CourseGroups.ToListAsync();
    }

    // It create select list for html
    public async Task<SelectList> GetGroupsForManageCourse()
    {
        // SelectList need list of SelectListItem
        var selectListItems = await _context.CourseGroups.Where(g => g.ParentId == null)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString(),
                })
                .ToListAsync();

        // It can render to SelectList
        var selectList = new SelectList(selectListItems, "Value", "Text");

        return selectList;
    }

    // It create select list for html
    public async Task<SelectList> GetSubGroupsForManageCourse(int parentGroupId)
    {
        var firstSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "انتخاب کنید", Value = "" } };

        // SelectList need list of SelectListItem
        var dbSelectListItems = await _context.CourseGroups.Where(g => g.ParentId == parentGroupId)
            .Select(g => new SelectListItem()
            {
                Text = g.GroupTitle,
                Value = g.GroupId.ToString(),
            })
            .ToListAsync();

        var selectListItems = firstSelectListItem.Concat(dbSelectListItems);

        // It can render to SelectList
        var selectList = new SelectList(selectListItems, "Value", "Text");

        return selectList;
    }

    public async Task<SelectList> GetAllTeachers()
    {
        // SelectList need list of SelectListItem
        var selectListItems = await _context.UserRoles.Where(u => u.RoleId == TeacherRoleId)
                                        .Include(u => u.User)
                                        .Select(u => new SelectListItem()
                                        {
                                            Value = u.UserId.ToString(),
                                            Text = u.User.UserName,
                                        })
                                        .ToListAsync();
        // SelectListItem can render to SelectList
        var selectList = new SelectList(selectListItems, "Value", "Text");

        return selectList;
    }

    public async Task<SelectList> GetAllLevels()
    {
        // SelectList need list of SelectListItem
        var selectListItems = await _context.CourseLevels
            .Select(l => new SelectListItem()
            {
                Value = l.LevelId.ToString(),
                Text = l.LevelTitle,
            })
            .ToListAsync();
        // SelectListItem can render to SelectList
        var selectList = new SelectList(selectListItems, "Value", "Text");

        return selectList;
    }

    public async Task<SelectList> GetAllStatus()
    {
        // SelectList need list of SelectListItem
        var selectListItems = await _context.CourseStatus
            .Select(s => new SelectListItem()
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusTitle,
            })
            .ToListAsync();
        // SelectListItem can render to SelectList
        var selectList = new SelectList(selectListItems, "Value", "Text");

        return selectList;
    }

    public async Task<int> CreateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp)
    {
        course.CreateDateTime = DateTime.Now;
        course.CourseImageName = "Default.png";

        // We had new image to upload
        if (imgCourseUp != null && await _imageUtils.ImageIsValid(imgCourseUp))
        {
            // Save new image
            course.CourseImageName =
                $"{CodeGenerators.Generate32ByteUniqueCode()}{Path.GetExtension(imgCourseUp.FileName)}";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Images",
                course.CourseImageName);
            // This using is auto dispose!
            await using (var stream = new FileStream(imagePath, FileMode.Create))
                await imgCourseUp.CopyToAsync(stream);

            // Create resized thumbnail
            var thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Thumb", course.CourseImageName);
            await _imageUtils.ImageResize(imagePath, thumbPath, 150, 150);
        }

        // Demo
        if (demoUp != null)
        {
            // Save new image
            course.DemoFileName =
                $"{CodeGenerators.Generate32ByteUniqueCode()}{Path.GetExtension(demoUp.FileName)}";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Demos",
                course.DemoFileName);
            // This using is auto dispose!
            await using (var stream = new FileStream(imagePath, FileMode.Create))
                await demoUp.CopyToAsync(stream);
        }

        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        return course.CourseId;
    }

    public async Task<List<ShowCourseInAdminViewModel>> GetAllCoursesForAdmin()
    {
        return await _context.Courses.Select(c => new ShowCourseInAdminViewModel()
        {
            CourseId = c.CourseId,
            ImageName = c.CourseImageName,
            Title = c.CourseTitle,
            EpisodeCount = c.CourseEpisodes.Count
        }).ToListAsync();
    }
}