using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class CourseService : ICourseService
{
    private readonly MssqlContext _context;

    public CourseService(MssqlContext context)
    {
        _context = context;
    }

    public async Task<List<CourseGroup>> GetAll()
    {
        return await _context.CourseGroups.ToListAsync();
    }
}