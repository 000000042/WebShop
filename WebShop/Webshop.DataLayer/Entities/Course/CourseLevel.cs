using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Webshop.DataLayer.Entities.Course
{
    public class CourseLevel
    {
        [Key]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "سطح دوره")]
        public string LevelTitle { get; set; }

        public List<Course> Courses { get; set; }
    }
}
