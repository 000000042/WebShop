using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.DTOs.Course
{
    public class ShowCoursesViewModel
    {
        public int CourseId { get; set; }

        public string CourseTitle { get; set; }

        public int CoursePrice { get; set; }

        public TimeSpan CourseTime { get; set; }

        public string CourseImageName { get; set; }
    }
}
