using CourseSeller.DataLayer.Entities.Courses;

namespace CourseSeller.Core.Services.Interfaces;

public interface ICourseService
{
    #region Group

    Task<List<CourseGroup>> GetAll();

    #endregion
}