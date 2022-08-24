using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GameShop.DataLayer.Entities.Course
{
    public class UserCourse
    {
        [Key]
        public int UC_Id { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }


        #region Navigation Props

        public User.User User { get; set; }

        public Course Course { get; set; }

        #endregion
    }
}
