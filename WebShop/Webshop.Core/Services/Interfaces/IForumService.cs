using Webshop.Core.DTOs.Question;
using Webshop.DataLayer.Entities.QustionAnswer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.Core.Services.Interfaces
{
    public interface IForumService
    {
        #region Question

        int CreateQuestion(Question question, string userName);

        ShowQuestionVM ShowQuestion(int questionId);

        List<Question> FilterQuestions(int? courseId, string filter = "");

        #endregion

        #region Answer

        int CreateAnswer(int questionId, string userName, string bodyAnswer);

        int SelectCorrectAnswer(int answerId, string userName);

        #endregion
    }
}
