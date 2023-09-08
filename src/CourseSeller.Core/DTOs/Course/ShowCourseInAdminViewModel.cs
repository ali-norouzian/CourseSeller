namespace CourseSeller.Core.DTOs.Course
{
    public class ShowCourseInAdminViewModel
    {
        public int CourseId { get; set; }

        public string Title { get; set; }

        public string ImageName { get; set; }

        public int EpisodeCount { get; set; }

        public int StudentCount { get; set; }
    }
}
