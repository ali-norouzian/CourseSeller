using CourseSeller.Core.DTOs.Course;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSeller.Core.Services.Interfaces;

public interface ICourseService
{
    #region Group

    Task<List<CourseGroup>> GetAll();
    Task<SelectList> GetGroupsForManageCourse();
    Task<SelectList> GetSubGroupsForManageCourse(int parentGroupId);
    Task<SelectList> GetAllTeachers();
    Task<SelectList> GetAllLevels();
    Task<SelectList> GetAllStatus();
    Task<int> CreateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp);
    Task<List<ShowCourseInAdminViewModel>> GetAllCoursesForAdmin();

    #endregion
}