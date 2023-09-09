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


    public const string TypeForAll = "all";
    public const string TypeForBuy = "buy";
    public const string TypeForFree = "free";

    public const string OrderByDate = "date";
    public const string OrderByUpdateDate = "updatedate";
    public const string OrderByPrice = "price";


    private readonly MssqlContext _context;
    private readonly IImageUtils _imageUtils;

    public CourseService(MssqlContext context, IImageUtils imageUtils)
    {
        _context = context;
        _imageUtils = imageUtils;
    }

    public async Task<List<CourseGroup>> GetAllGroups()
    {
        return await _context.CourseGroups
            // To having subgroups that related:
            .Include(c => c.CourseGroups)
            .ToListAsync();
    }

    // It create select list for html
    public async Task<SelectList> GetGroupsForManageCourse(int? selectedId = null)
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
        var selectList = new SelectList(selectListItems, "Value", "Text", selectedId);

        return selectList;
    }

    // It create select list for html
    public async Task<SelectList> GetSubGroupsForManageCourse(int parentGroupId, int? selectedId = null)
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
        var selectList = new SelectList(selectListItems, "Value", "Text", selectedId);

        return selectList;
    }

    public async Task<SelectList> GetAllTeachers(string? selectedId = null)
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
        var selectList = new SelectList(selectListItems, "Value", "Text", selectedId);

        return selectList;
    }

    public async Task<SelectList> GetAllLevels(int? selectedId = null)
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
        var selectList = new SelectList(selectListItems, "Value", "Text", selectedId);

        return selectList;
    }

    public async Task<SelectList> GetAllStatus(int? selectedId = null)
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
        var selectList = new SelectList(selectListItems, "Value", "Text", selectedId);

        return selectList;
    }

    public async Task AddGroup(CourseGroup group)
    {
        await _context.CourseGroups.AddAsync(group);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateGroup(CourseGroup group)
    {
        _context.CourseGroups.Update(group);
        await _context.SaveChangesAsync();
    }

    public async Task<CourseGroup> GetGroup(int? groupId)
    {
        return await _context.CourseGroups.FindAsync(groupId);
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
        // If you want access to for example count of students of each
        //  course here you most use group by to.
        return await _context.Courses.Select(c => new ShowCourseInAdminViewModel()
        {
            CourseId = c.CourseId,
            ImageName = c.CourseImageName,
            Title = c.CourseTitle,
            EpisodeCount = c.CourseEpisodes.Count,
            StudentCount = c.UserCourses.Count
        }).ToListAsync();
    }

    public async Task<Course> GetCourseById(int id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task UpdateCourse(Course course, IFormFile imgCourseUp, IFormFile demoUp)
    {
        course.UpdateDateTime = DateTime.Now;

        // We had new image to upload
        if (imgCourseUp != null && await _imageUtils.ImageIsValid(imgCourseUp))
        {
            if (course.CourseImageName != "Default.png")
            {
                var deleteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Images",
                    course.CourseImageName);
                // We can do soft delete and hold use old images in a folder for security purpose
                // BUG: We have roleback db on error but we havent it on delete file!
                if (File.Exists(deleteImagePath))
                    File.Delete(deleteImagePath);

                var deleteThumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Thumb",
                    course.CourseImageName);
                // We can do soft delete and hold use old images in a folder for security purpose
                // BUG: We have roleback db on error but we havent it on delete file!
                if (File.Exists(deleteThumbPath))
                    File.Delete(deleteThumbPath);
            }

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
            if (course.DemoFileName != null)
            {
                var deleteDemoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Demos",
                    course.DemoFileName);
                // We can do soft delete and hold use old images in a folder for security purpose
                // BUG: We have roleback db on error but we havent it on delete file!
                if (File.Exists(deleteDemoPath))
                    File.Delete(deleteDemoPath);
            }

            // Save new image
            course.DemoFileName =
                $"{CodeGenerators.Generate32ByteUniqueCode()}{Path.GetExtension(demoUp.FileName)}";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Demos",
                course.DemoFileName);
            // This using is auto dispose!
            await using (var stream = new FileStream(imagePath, FileMode.Create))
                await demoUp.CopyToAsync(stream);
        }


        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    // list of courses, pageCount, maxPrice, minPrice
    public async Task<Tuple<List<ShowCourseForListViewModel>, int, int, int>> GetAllCourse(int pageId = 1, int take = 8, string filter = null,
        string getType = TypeForAll, string orderByType = OrderByDate,
        int startPrice = 0, int endPrice = int.MaxValue, List<int> selectedGroups = null)
    {
        IQueryable<Course> result = _context.Courses;

        if (!string.IsNullOrEmpty(filter))
            result = result.Where(c => c.CourseTitle.Contains(filter) || c.Tags.Contains(filter));

        switch (getType)
        {
            case TypeForAll:
                break;
            case TypeForBuy:
                result = result.Where(c => c.CoursePrice != 0);
                break;
            case TypeForFree:
                result = result.Where(c => c.CoursePrice == 0);
                break;
        }

        switch (orderByType)
        {
            case OrderByDate:
                result = result.OrderByDescending(c => c.CreateDateTime);
                break;
            case OrderByUpdateDate:
                result = result.OrderByDescending(c => c.UpdateDateTime);
                break;
            case OrderByPrice:
                result = result.OrderByDescending(c => c.CoursePrice);
                break;
        }

        if (0 < startPrice)
            result = result.Where(c => c.CoursePrice > startPrice);
        if (endPrice < int.MaxValue)
            result = result.Where(c => c.CoursePrice < endPrice);

        if (selectedGroups != null && selectedGroups.Any())
        {
            foreach (var groupId in selectedGroups)
            {
                result = result.Where(c => c.GroupId == groupId || c.SubGroupId == groupId);
            }
        }


        /*
         * Performance:
         *  If the collection is large and you want to avoid blocking the current thread,
         *  the first code using `asAsyncEnumerable` may be more efficient.
         * This AsAsyncEnumerable open a stream and one by one timespan can calculate.
         */
        int skip = (pageId - 1) * take;
        var viewModel = result
            .AsQueryable()
            .Include(c => c.CourseEpisodes)
            .Skip(skip)
            .Take(take)
            .AsAsyncEnumerable();

        var returnResult = new List<ShowCourseForListViewModel>();
        await foreach (var course in viewModel)
        {
            returnResult.Add(new ShowCourseForListViewModel()
            {
                CourseId = course.CourseId,
                ImageName = course.CourseImageName,
                Price = course.CoursePrice,
                Title = course.CourseTitle,
                TotalTime = new TimeSpan(course.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
            });
        }

        /*
         * .AsEnumerable()
         * .select(c => new ShowCourseForListViewModel()
           {
           CourseId = c.CourseId,
           ImageName = c.CourseImageName,
           Price = c.CoursePrice,
           Title = c.CourseTitle,
           TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks)),
           }).ToList();
         */


        // 8 take is for home page and in home we don't need this query
        int pageCount = 0;
        if (take != 8)
            pageCount = await GetCountOfFilteredCourse(result, take);

        // For havent extra query in first home page
        int maxPrice = 0, minPrice = 0;
        if (!(0 < startPrice && endPrice < int.MaxValue))
        {
            try
            {
                maxPrice = await result.MaxAsync(c => c.CoursePrice);
                minPrice = await result.MinAsync(c => c.CoursePrice);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return Tuple.Create(returnResult, pageCount, maxPrice, minPrice);
    }

    public async Task<int> GetCountOfFilteredCourse(IQueryable<Course> q, int takeEachPage)
    {
        return await q
            .Include(c => c.CourseEpisodes)
            .CountAsync() / takeEachPage;
    }

    public async Task<Course> GetCourseByForShowSingle(int courseId)
    {
        return await _context.Courses
            .Include(c => c.CourseEpisodes)
            .Include(c => c.CourseStatus)
            .Include(c => c.CourseLevel)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);
    }

    public async Task<int> GetStudentsCountOfCourse(int courseId)
    {
        return await _context.UserCourses.CountAsync(c => c.CourseId == courseId);
    }

    public async Task<List<ShowCourseForListViewModel>> GetMostPopularCourses()
    {
        var courses = await _context.Courses
            .Include(c => c.OrderDetails)
            .Include(c => c.CourseEpisodes)
            .Where(c => c.OrderDetails.Any())
            .OrderByDescending(c => c.OrderDetails.Count())
            .Take(8)
            .ToListAsync();

        return courses.Select(c => new ShowCourseForListViewModel()
        {
            CourseId = c.CourseId,
            ImageName = c.CourseImageName,
            Price = c.CoursePrice,
            Title = c.CourseTitle,
            TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
        }).ToList();
    }


    public async Task<List<CourseEpisode>> ListCourseEpisodes(int courseId)
    {
        return await _context.CourseEpisodes
            .Where(c => c.CourseId == courseId)
            .ToListAsync();
    }

    // Todo: episodes files can be foldered by each course
    public async Task<int> CreateEpisode(CourseEpisode episode, IFormFile episodeFile)
    {
        episode.EpisodeFileName = episodeFile.FileName;

        // Save new image
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Episodes",
            episode.EpisodeFileName);
        // This using is auto dispose!
        await using (var stream = new FileStream(filePath, FileMode.Create))
            await episodeFile.CopyToAsync(stream);

        await _context.CourseEpisodes.AddAsync(episode);
        await _context.SaveChangesAsync();

        return episode.EpisodeId;
    }

    public async Task<bool> CheckExistFile(string fileName, string filePath = "wwwroot/Courses/Episodes")
    {
        filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath,
            fileName);

        return File.Exists(filePath);
    }

    public async Task<CourseEpisode> GetEpisodeById(int id)
    {
        return await _context.CourseEpisodes.FindAsync(id);
    }

    public async Task UpdateEpisode(CourseEpisode episode, IFormFile episodeFile)
    {
        if (episodeFile != null)
        {
            var deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Episodes",
                episode.EpisodeFileName);
            File.Delete(deleteFilePath);

            episode.EpisodeFileName = episodeFile.FileName;
            // Save new image
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Courses/Episodes",
                episode.EpisodeFileName);
            // This using is auto dispose!
            await using (var stream = new FileStream(filePath, FileMode.Create))
                await episodeFile.CopyToAsync(stream);
        }

        _context.CourseEpisodes.Update(episode);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EpisodeIsFree(int episodeId)
    {
        return await _context.CourseEpisodes.AnyAsync(e => e.EpisodeId == episodeId && e.IsFree == true);
    }

    public async Task CreateComment(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<Comment>, int)> GetCourseComment(int courseId, int pageId = 1)
    {
        var take = 5;
        var skip = (pageId - 1) * take;

        var commentCounts = await _context.Comments
            .Where(c => c.CourseId == courseId && c.IsDelete == false)
            .CountAsync();

        // For example: 14/5=2.8... but we need 3 pageination. so use cail
        var pageCount = (int)(Math.Ceiling((double)commentCounts / take));

        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.CourseId == courseId && c.IsDelete == false)
            .OrderByDescending(c => c.CreateDateTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (comments, pageCount);
    }

    public async Task CreateVote(string userId, int courseId, bool vote)
    {
        var existedVote = await _context.CourseVotes
            .FirstOrDefaultAsync(v => v.UserId == userId && v.CourseId == courseId);
        // if it exist update it
        if (existedVote != null)
        {
            existedVote.vote = vote;
        }
        else
        {
            existedVote = new CourseVote()
            {
                CourseId = courseId,
                UserId = userId,
                vote = vote,
            };
            await _context.AddAsync(existedVote);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<(int, int)> GetCourseVote(int courseId)
    {
        var vote = await _context.CourseVotes
            .Where(v => v.CourseId == courseId)
            .Select(v => v.vote)
            .ToListAsync();

        return (vote.Count(v => v), vote.Count(v => !v));
    }

    public async Task<bool> IsFree(int courseId)
    {
        return await _context.Courses
            .AnyAsync(c => c.CourseId == courseId && c.CoursePrice == 0);

    }
}