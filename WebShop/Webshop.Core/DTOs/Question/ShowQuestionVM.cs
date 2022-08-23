using Webshop.DataLayer.Entities.QustionAnswer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.Core.DTOs.Question
{
    public class ShowQuestionVM
    {
        public DataLayer.Entities.QustionAnswer.Question Question { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
