using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.DTOs.Course
{
    public class ShowCourseViewModel
    {
        public List<CourseViewModel> CourseList { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public string StatusTitle { get; set; }

        public int CourseEpisodes { get; set; }
    }

    public class CourseViewModel
    {
        public int CourseId { get; set; }

        public string CourseTitle { get; set; }

        public string CourseStatus { get; set; }

        public string CourseImageName { get; set; }

        public int CourseEpisodes { get; set; }

        public DateTime CourseCreateDate { get; set; }
    }

    public class DeleteCourseViewModel
    {
        public int CourseId { get; set; }

        public string CourseTitle { get; set; }

        public string CourseStatus { get; set; }

        public string CourseLevel { get; set; }

        public string CourseImageName { get; set; }

        public int CoursePrice { get; set; }

        public DateTime CourseCreateDate { get; set; }

        public int CourseEpisodes { get; set; }

        public string CourseGroupName { get; set; }

        public string CourseSubGroupName { get; set; }

        public string CourseDescription { get; set; }
    }
}
