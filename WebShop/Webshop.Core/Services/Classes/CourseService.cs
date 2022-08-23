using Webshop.Core.Conventors;
using Webshop.Core.DTOs.Course;
using Webshop.Core.Generators;
using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using Webshop.DataLayer.Entities.Course;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Webshop.Core.Security;

namespace Webshop.Core.Services.Classes
{
    public class CourseService : ICourseService
    {
        private WebshopContext _context;

        public CourseService(WebshopContext context)
        {
            _context = context;
        }

        public int AddCourse(Course course, IFormFile imgCourse, IFormFile demoCourse)
        {
            course.CreateDate = DateTime.Now;

            //ToDO Save Image
            course.CourseImageName = "DefaultImage.jpg";
            if (imgCourse != null && imgCourse.IsImageValid())
            {
                course.CourseImageName = GuidGenerator.ActiveCodeGenerator() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/image/" + course.CourseImageName);

                using (FileStream stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                //TODO Resize Image

                ImageConvertor imageResizer = new ImageConvertor();

                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/thumb/" + course.CourseImageName);

                imageResizer.Image_resize(imagePath, thumbPath, 120);
            }

            //ToDO Upload demo file

            if (demoCourse != null)
            {
                course.DemoFileName = GuidGenerator.ActiveCodeGenerator() + Path.GetExtension(demoCourse.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/demoes/" + course.DemoFileName);

                using (FileStream stream = new FileStream(demoPath, FileMode.Create))
                {
                    demoCourse.CopyTo(stream);
                }
            }


            //ToDO Save course
            _context.Courses.Add(course);
            _context.SaveChanges();

            return course.CourseId;
        }

        public bool CheckExistFile(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/episodes", fileName);
            return File.Exists(path);
        }

        public int AddEpisode(CourseEpisode episode, IFormFile fileEpisode)
        {
            if (fileEpisode != null && !CheckExistFile(fileEpisode.FileName))
            {
                episode.EpisodeFileName = fileEpisode.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/episodes", fileEpisode.FileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    fileEpisode.CopyTo(stream);
                }
            }

            _context.CourseEpisodes.Add(episode);
            _context.SaveChanges();

            return episode.EpisodeId;
        }

        public void DeleteCourse(int courseId)
        {
            Course course = _context.Courses.SingleOrDefault(c => c.CourseId == courseId);
            course.IsDelete = true;
            _context.Update(course);
            _context.SaveChanges();
        }

        public List<SelectListItem> GetAllCourseLevelsToSelectList()
        {
            return _context.CourseLevels
                .Select(l => new SelectListItem()
                {
                    Text = l.LevelTitle,
                    Value = l.LevelId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetAllCourseStatuesToSelectList()
        {
            return _context.CourseStatuses
                .Select(s => new SelectListItem()
                {
                    Text = s.StatusTitle,
                    Value = s.StatusId.ToString()
                }).ToList();
        }

        public List<CourseGroup> GetAllGroups()
        {
            return _context.CourseGroups.ToList();
        }

        public List<SelectListItem> GetAllGroupsToSelectList()
        {
            return _context.CourseGroups.Where(c => c.ParentId == null)
                .Select(c => new SelectListItem()
                {
                    Text = c.GroupTitle,
                    Value = c.CourseGroupId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetAllSubGroupsToSelectList(int groupId)
        {
            return _context.CourseGroups.Where(c => c.ParentId == groupId)
                .Select(c => new SelectListItem()
                {
                    Text = c.GroupTitle,
                    Value = c.CourseGroupId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetAllTeachersToSelectList()
        {
            return _context.UserRoles.Include(u => u.User).Where(r => r.RoleId == 2)
                .Select(u => new SelectListItem()
                {
                    Text = u.User.UserName,
                    Value = u.UserId.ToString()
                }).ToList();
        }

        public Course GetCourseById(int courseId)
        {
            return _context.Courses.SingleOrDefault(c => c.CourseId == courseId);
        }

        public DeleteCourseViewModel GetCourseForDelete(int courseId)
        {
            return _context.Courses.Where(c => c.CourseId == courseId)
                .Include(s => s.CourseStatus)
                .Include(l => l.CourseLevel)
                .Include(e => e.CourseEpisodes)
                .Include(g => g.CourseGroup)
                .Select(c => new DeleteCourseViewModel()
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.CourseTitle,
                    CourseStatus = c.CourseStatus.StatusTitle,
                    CourseLevel = c.CourseLevel.LevelTitle,
                    CourseCreateDate = c.CreateDate,
                    CourseImageName = c.CourseImageName,
                    CourseEpisodes = c.CourseEpisodes.Count(),
                    CoursePrice = c.CoursePrice,
                    CourseGroupName = c.CourseGroup.GroupTitle,
                    CourseSubGroupName = c.Group.GroupTitle,
                    CourseDescription = c.CourseDescription
                }).SingleOrDefault();
        }

        public ShowCourseViewModel GetCoursesToShow(int pageId = 1)
        {
            int take = 5;
            int skip = (pageId - 1) * take;

            var Courses = _context.Courses.Include(s => s.CourseStatus)
                .Include(e => e.CourseEpisodes)
                .Skip(skip).Take(take)
                .Select(c => new CourseViewModel()
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.CourseTitle,
                    CourseCreateDate = c.CreateDate,
                    CourseStatus = c.CourseStatus.StatusTitle,
                    CourseEpisodes = c.CourseEpisodes.Count(),
                    CourseImageName = c.CourseImageName
                }).OrderBy(c => c.CourseCreateDate).ToList();

            ShowCourseViewModel ShowCourses = new ShowCourseViewModel();
            ShowCourses.CurrentPage = pageId;
            ShowCourses.PageCount = Courses.Count() / take;

            ShowCourses.CourseList = Courses;
            return ShowCourses;
        }

        public List<CourseEpisode> GetEpisodesByCourseId(int courseId)
        {
            return _context.CourseEpisodes.Include(c => c.Course).Where(c => c.CourseId == courseId).ToList();
        }

        public void UpdateCourse(Course course, IFormFile imgCourse, IFormFile demoCourse)
        {
            course.UpdateDate = DateTime.Now;

            //ToDO Save Image

            if (imgCourse != null && imgCourse.IsImageValid())
            {
                if (course.CourseImageName != "DefaultImage.jpg")
                {
                    string deleteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/image/" + course.CourseImageName);
                    if (File.Exists(deleteImagePath))
                    {
                        File.Delete(deleteImagePath);
                    }

                    string deleteThumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/thumb/" + course.CourseImageName);

                    if (File.Exists(deleteThumbPath))
                    {
                        File.Delete(deleteThumbPath);
                    }
                }

                course.CourseImageName = GuidGenerator.ActiveCodeGenerator() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/image/" + course.CourseImageName);

                using (FileStream stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                //TODO Resize Image

                ImageConvertor imageResizer = new ImageConvertor();

                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/thumb/" + course.CourseImageName);

                imageResizer.Image_resize(imagePath, thumbPath, 120);
            }

            //ToDO Upload demo file

            if (demoCourse != null)
            {
                if (course.DemoFileName != null)
                {
                    string deleteDemoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/demoes/" + course.DemoFileName);
                    if (File.Exists(deleteDemoPath))
                    {
                        File.Delete(deleteDemoPath);
                    }
                }

                course.DemoFileName = GuidGenerator.ActiveCodeGenerator() + Path.GetExtension(demoCourse.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/demoes/" + course.DemoFileName);

                using (FileStream stream = new FileStream(demoPath, FileMode.Create))
                {
                    demoCourse.CopyTo(stream);
                }
            }

            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public CourseEpisode GetEpisodeById(int episodeId)
        {
            return _context.CourseEpisodes.SingleOrDefault(e => e.EpisodeId == episodeId);
        }

        public void EditEpisode(CourseEpisode episode, IFormFile fileEpisode)
        {
            if (fileEpisode != null)
            {
                string deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/episodes", episode.EpisodeFileName);

                if (File.Exists(deleteFilePath))
                {
                    File.Delete(deleteFilePath);
                }

                episode.EpisodeFileName = fileEpisode.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/episodes", fileEpisode.FileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    fileEpisode.CopyTo(stream);
                }
            }

            _context.CourseEpisodes.Update(episode);
            _context.SaveChanges();
        }

        public int DeleteEpisode(int episodeId)
        {
            CourseEpisode episode = _context.CourseEpisodes.SingleOrDefault(e => e.EpisodeId == episodeId);
            int courseId = episode.CourseId;
            _context.Remove(episode);
            _context.SaveChanges();

            return courseId;
        }

        public Tuple<List<ShowCoursesViewModel>, int> GetCoursesForShow(int pageId = 1, string filterName = "", string filterCourseType = "all",
            string orderByCourse = "date", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0)
        {
            if (take == 0)
                take = 8;

            IQueryable<Course> result = _context.Courses;

            if (!string.IsNullOrEmpty(filterName))
            {
                result = result.Where(c => c.CourseTitle.Contains(filterName) || c.Tags.Contains(filterName));
            }

            switch (filterCourseType)
            {
                case "all":
                    break;

                case "saleable":
                    result = result.Where((c) => c.CoursePrice > 0);
                    break;

                case "free":
                    result = result.Where((c) => c.CoursePrice == 0);
                    break;
            }

            switch (orderByCourse)
            {
                case "date":
                    result = result.OrderBy(c => c.CreateDate);
                    break;

                case "highPrice":
                    result = result.OrderBy(c => c.CoursePrice);
                    break;

                case "lowPrice":
                    result = result.OrderByDescending(c => c.CoursePrice);
                    break;

                case "update":
                    result = result.OrderBy(c => c.UpdateDate);
                    break;
            }

            if (startPrice > 0)
            {
                result = result.Where(c => c.CoursePrice > startPrice);
            }

            if (endPrice > 0)
            {
                result = result.Where(c => c.CoursePrice < startPrice);
            }

            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (var id in selectedGroups)
                {
                    result = result.Where(c => c.GroupId == id || c.SubGroup == id);
                }
            }

            int skip = (pageId - 1) * take;

            int pageCount = result.Select(c => new ShowCoursesViewModel()
            {
                CourseId = c.CourseId,
                CourseTitle = c.CourseTitle,
                CoursePrice = c.CoursePrice,
                CourseImageName = c.CourseImageName
            }).Count() / take;

            var query = result.Select(c => new ShowCoursesViewModel()
            {
                CourseId = c.CourseId,
                CourseTitle = c.CourseTitle,
                CoursePrice = c.CoursePrice,
                CourseImageName = c.CourseImageName
            }).Skip(skip).Take(take).ToList();

            return Tuple.Create(query, pageCount);
        }

        public Course GetCourseForShowCourse(int courseId)
        {
            return _context.Courses
            .Include(c => c.CourseEpisodes)
            .Include(c => c.CourseStatus)
            .Include(c => c.CourseLevel)
            .Include(c => c.User)
            .FirstOrDefault(c => c.CourseId == courseId);
        }

        public void AddComment(CourseComment comment)
        {
            _context.CourseComments.Add(comment);
            _context.SaveChanges();
        }

        public Tuple<List<CourseComment>, int> GetCommentsForCourse(int courseId, int pageId = 1)
        {
            int take = 5;
            int skip = (pageId - 1) * take;

            int pageCount = _context.CourseComments.Where(c => c.CourseId == courseId && !c.IsDelete).Count() / take;

            if ((pageCount % 2) != 0)
                pageCount+= 1;

            return Tuple.Create(_context.CourseComments.Include(u => u.User).Where(c => c.CourseId == courseId && !c.IsDelete).Skip(skip).Take(take).ToList(), pageCount);
        }

        public void EditComment(CourseComment comment)
        {
            _context.CourseComments.Update(comment);
            _context.SaveChanges();
        }

        public void DeleteComment(int commentId)
        {
            CourseComment comment = _context.CourseComments.SingleOrDefault(c => !c.IsDelete && c.CommentId == commentId);
            comment.IsDelete = true;
            _context.CourseComments.Update(comment);
            _context.SaveChanges();
        }

        public List<ShowCoursesViewModel> GetPopularCoursesToShow()
        {
            return _context.Courses
                .Include(od => od.OrderDetails)
                .Include(e => e.CourseEpisodes)
                .Where(c => c.OrderDetails.Any())
                .OrderByDescending(od => od.OrderDetails.Count())
                .Select(c => new ShowCoursesViewModel()
                {
                    CourseId = c.CourseId,
                    CourseImageName = c.CourseImageName,
                    CoursePrice = c.CoursePrice,
                    CourseTitle = c.CourseTitle
                }).ToList();
        }

        public void CreateGroup(CourseGroup group)
        {
            _context.CourseGroups.Add(group);
            _context.SaveChanges();
        }

        public Tuple<List<CourseGroup>, List<CourseGroup>, int> GetGroupsForShow(int pageId = 1)
        {
            IQueryable<CourseGroup> groups = _context.CourseGroups.Include(sg => sg.CourseGroups).Where(g => !g.IsDelete);
            IQueryable<CourseGroup> subGroups = _context.CourseGroups.Include(sg => sg.CourseGroups).Where(sg => sg.ParentId != null && !sg.IsDelete);
            int take = 5;
            int skip = (pageId - 1) * take;
            int pageCount = groups.Where(g => g.ParentId == null).Count() / take;
            if ((pageCount % 5) != 0)
                pageCount += 1;

            return Tuple.Create(groups.Where(g => g.ParentId == null).Skip(skip).Take(take).ToList(), subGroups.ToList(), pageCount);
        }

        public void UpdateGroup(CourseGroup group)
        {
            _context.CourseGroups.Update(group);
            _context.SaveChanges();
        }

        public void DeleteGroup(CourseGroup group)
        {
            group.IsDelete = true;
            _context.CourseGroups.Update(group);
            _context.SaveChanges();
        }

        public CourseGroup GetGroupById(int groupId)
        {
            return _context.CourseGroups.Find(groupId);
        }

        public void AddVote(int userId, int courseId, bool vote)
        {
            var userVote = _context.CourseVotes.SingleOrDefault(v => v.UserId == userId && v.CourseId == courseId);
            if (userVote == null)
            {
                userVote = new CourseVote()
                {
                    UserId = userId,
                    CourseId = courseId,
                    Vote = vote
                };

                _context.CourseVotes.Add(userVote);
            }
            else
            {
                userVote.Vote = vote;
                _context.CourseVotes.Update(userVote);
            }
            _context.SaveChanges();
        }

        public bool IsUserVoted(int userId, int courseId)
        {
            return _context.CourseVotes.Any(v => v.UserId == userId && v.CourseId == courseId);
        }

        public Tuple<int, int> VoteCount(int courseId)
        {
            var votes = _context.CourseVotes.Where(v => v.CourseId == courseId).Select(v => v.Vote);

            return Tuple.Create(votes.Count(c => c), votes.Count(c => !c));
        }

        public bool IsCourseFree(int courseId)
        {
            return _context.Courses.Where(c => c.CourseId == courseId).Select(c => c.CoursePrice).FirstOrDefault() == 0;
        }

        public List<string> GetCourseTitles(string term)
        {
            return _context.Courses.Where(c => c.CourseTitle.Contains(term))
                .Select(c => c.CourseTitle)
                .ToList();
        }

        public List<Course> GetCoursesForMaster(string userName)
        {
            int userId = _context.Users.SingleOrDefault(u => u.UserName == userName).UserId;

            var courses = _context.Courses.Where(c => c.TeacherId == userId)
                .Include(s => s.CourseStatus)
                .Include(e=> e.CourseEpisodes)
                .ToList();

            return courses;
        }
    }
}
