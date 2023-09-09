using CourseSeller.Core.DTOs.Course;
using CourseSeller.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using static CourseSeller.Core.Services.CourseService;

namespace CourseSeller.Core.Services.Interfaces;

public interface ICourseService
{
    #region Group

    Task<List<CourseGroup>> GetAllGroups();
    Task<SelectList> GetGroupsForManageCourse(int? selectedId = null);
    Task<SelectList> GetSubGroupsForManageCourse(int parentGroupId, int? selectedId = null);
    Task<SelectList> GetAllTeachers(string? selectedId = null);
    Task<SelectList> GetAllLevels(int? selectedId = null);
    Task<SelectList> GetAllStatus(int? selectedId = null);
    Task AddGroup(CourseGroup group);
    Task UpdateGroup(CourseGroup group);
    Task<CourseGroup> GetGroup(int? groupId);

    #endregion


    #region Course

    Task<int> CreateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp);
    Task<List<ShowCourseInAdminViewModel>> GetAllCoursesForAdmin();
    Task<Course> GetCourseById(int id);
    Task UpdateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp);
    Task<Tuple<List<ShowCourseForListViewModel>, int, int, int>> GetAllCourse(int pageId = 1, int take = 8, string filter = null,
        string getType = TypeForAll, string orderByType = OrderByDate,
        int startPrice = 0, int endPrice = int.MaxValue, List<int> selectedGroups = null);
    Task<int> GetCountOfFilteredCourse(IQueryable<Course> q, int takeEachPage);
    Task<Course> GetCourseByForShowSingle(int courseId);
    Task<int> GetStudentsCountOfCourse(int courseId);
    Task<List<ShowCourseForListViewModel>> GetMostPopularCourses();

    #endregion


    #region Episodes

    Task<List<CourseEpisode>> ListCourseEpisodes(int courseId);
    Task<int> CreateEpisode(CourseEpisode episode, IFormFile episodeFile);
    Task<bool> CheckExistFile(string fileName, string filePath = "wwwroot/Courses/Episodes");
    Task<CourseEpisode> GetEpisodeById(int id);
    Task UpdateEpisode(CourseEpisode episode, IFormFile episodeFile);

    #endregion


    #region Comment

    Task CreateComment(Comment comment);
    Task<(List<Comment>, int)> GetCourseComment(int courseId, int pageId = 1);

    #endregion

}