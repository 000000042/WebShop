using Webshop.Core.DTOs.Course;
using Webshop.DataLayer.Entities.Course;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.Services.Interfaces
{
    public interface ICourseService
    {
        #region Courses

        List<CourseGroup> GetAllGroups();

        List<SelectListItem> GetAllGroupsToSelectList();

        List<SelectListItem> GetAllSubGroupsToSelectList(int groupId);

        List<SelectListItem> GetAllTeachersToSelectList();

        List<SelectListItem> GetAllCourseStatuesToSelectList();

        List<SelectListItem> GetAllCourseLevelsToSelectList();

        int AddCourse(Course course, IFormFile imgCourse, IFormFile demoCourse);

        ShowCourseViewModel GetCoursesToShow(int pageId);

        Course GetCourseById(int courseId);

        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile demoCourse);

        DeleteCourseViewModel GetCourseForDelete(int courseId);

        void DeleteCourse(int courseId);

        Tuple<List<ShowCoursesViewModel>, int> GetCoursesForShow(int pageId = 1, string filterName = "",
            string filterCourseType = "all", string orderByCourse = "date", int startPrice = 0,
            int endPrice = 0, List<int> selectedGroups = null, int take = 0);

        Course GetCourseForShowCourse(int courseId);

        List<ShowCoursesViewModel> GetPopularCoursesToShow();

        bool IsCourseFree(int courseId);

        List<string> GetCourseTitles(string term);

        List<Course> GetCoursesForMaster(string userName);

        #endregion

        #region Episodes

        List<CourseEpisode> GetEpisodesByCourseId(int courseId);

        int AddEpisode(CourseEpisode episode, IFormFile fileEpisode);

        bool CheckExistFile(string fileName);

        CourseEpisode GetEpisodeById(int episodeId);

        void EditEpisode(CourseEpisode episode, IFormFile fileEpisode);

        int DeleteEpisode(int episodeId);

        #endregion

        #region Comments

        void AddComment(CourseComment comment);

        Tuple<List<CourseComment>, int> GetCommentsForCourse(int courseId, int pageId = 1);

        void EditComment(CourseComment comment);

        void DeleteComment(int commentId);

        #endregion

        #region Groups

        void CreateGroup(CourseGroup group);

        Tuple<List<CourseGroup>, List<CourseGroup>, int> GetGroupsForShow(int pageId = 1);

        void UpdateGroup(CourseGroup group);

        void DeleteGroup(CourseGroup group);

        CourseGroup GetGroupById(int groupId);

        #endregion

        #region Vote

        void AddVote(int userId, int courseId, bool vote);

        bool IsUserVoted(int userId, int courseId);

        Tuple<int, int> VoteCount(int courseId);

        #endregion
    }
}
