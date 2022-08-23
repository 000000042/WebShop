using Webshop.Core.DTOs.Question;
using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using Webshop.DataLayer.Entities.QustionAnswer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.Core.Services.Classes
{
    public class ForumService : IForumService
    {
        private WebshopContext _context;
        private IAccountService _accountService;

        public ForumService(WebshopContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public int CreateAnswer(int questionId, string userName, string bodyAnswer)
        {
            Answer answer = new Answer()
            {
                QuestionId = questionId,
                UserId = _accountService.GetUserIdByUserName(userName),
                Body = bodyAnswer,
                CreateDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            _context.Answers.Add(answer);
            _context.SaveChanges();

            return answer.QuestionId;
        }

        public int CreateQuestion(Question question, string userName)
        {
            question.CreateDate = DateTime.Now;
            question.ModifiedDate = DateTime.Now;

            int userId = _context.Users.SingleOrDefault(u => u.UserName == userName).UserId;
            question.UserId = userId;

            _context.Questions.Add(question);
            _context.SaveChanges();

            return question.QuestionId;
        }

        public List<Question> FilterQuestions(int? courseId, string filter = "")
        {
            IQueryable<Question> query = _context.Questions;

            if (courseId != null)
            {
                query = query.Where(q => q.CourseId == courseId);
            }

            if (filter != "")
            {
                query = query.Where(q => q.Title.Contains(filter));
            }

            return query.Include(u => u.User).Include(c => c.Course).Include(a => a.Answers).ToList();
        }

        public int SelectCorrectAnswer(int answerId, string userName)
        {
            Answer answer = _context.Answers.Include(q => q.Question).SingleOrDefault(u => u.AnswerId == answerId);
            if(answer != null)
            {
                int userId = _context.Users.SingleOrDefault(u => u.UserName == userName).UserId;
                if (userId == answer.Question.UserId)
                {
                    answer.IsCorrect = true;
                    _context.Answers.Update(answer);
                    _context.SaveChanges();

                    return answer.QuestionId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }

        public ShowQuestionVM ShowQuestion(int questionId)
        {
            ShowQuestionVM question = new ShowQuestionVM()
            {
                Question = _context.Questions.Include(u => u.User).SingleOrDefault(q => q.QuestionId == questionId),
                Answers = _context.Answers.Include(u => u.User).Where(a => a.QuestionId == questionId).ToList()
            };

            return question;
        }
    }
}
