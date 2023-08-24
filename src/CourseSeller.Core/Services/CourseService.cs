using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class CourseService : ICourseService
{
    public const byte TeacherRoleId = 2;

    private readonly MssqlContext _context;

    public CourseService(MssqlContext context)
    {
        _context = context;
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
}