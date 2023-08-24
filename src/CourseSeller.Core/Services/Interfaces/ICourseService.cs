using CourseSeller.DataLayer.Entities.Courses;
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

    #endregion
}