using CourseSeller.Core.DTOs.Course;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSeller.Core.Services.Interfaces;

public interface ICourseService
{
    #region Group

    Task<List<CourseGroup>> GetAll();
    Task<SelectList> GetGroupsForManageCourse(int? selectedId = null);
    Task<SelectList> GetSubGroupsForManageCourse(int parentGroupId, int? selectedId = null);
    Task<SelectList> GetAllTeachers(string? selectedId = null);
    Task<SelectList> GetAllLevels(int? selectedId = null);
    Task<SelectList> GetAllStatus(int? selectedId = null);
    Task<int> CreateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp);
    Task<List<ShowCourseInAdminViewModel>> GetAllCoursesForAdmin();
    Task<Course> GetCourseById(int id);
    Task UpdateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp);

    #endregion
}