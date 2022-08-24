using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.DataLayer.Entities.QustionAnswer
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [Required]
        public int UserId { get; set; }
        public User.User User { get; set; }

        [Required(ErrorMessage = "متن پاسخ را وارد نمایید!")]
        public string Body { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        public bool IsCorrect { get; set; }

        public bool IsDelete { get; set; }
    }
}
