using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.DataLayer.Entities.QustionAnswer
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "لطفا عنوان سوال را وارد نمایید!")]
        [Display(Name = "عنوان پرسش")]
        public string Title { get; set; }

        [Required(ErrorMessage = "متن پاسخ را وارد نمایید!")]
        [MaxLength(400)]
        [Display(Name = "متن پرسش")]
        public string Body { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        public bool IsDelete { get; set; }


        #region Navigation Props

        public User.User User { get; set; }

        public List<Answer> Answers { get; set; }

        public Course.Course Course { get; set; }

        #endregion
    }
}
