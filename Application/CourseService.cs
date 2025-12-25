using System.Collections.Generic;
using StudentSystem.Core;
using StudentSystem.Infrastructure;

namespace StudentSystem.Application
{
    public class CourseService
    {
        private readonly CourseRepository _repository;

        public CourseService()
        {
            _repository = new CourseRepository();
        }

        public List<Course> GetAllCourses() => _repository.GetAll();

        public void CreateCourse(string name, int credits, int deptId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.Exception("Course name cannot be empty!");

            var course = new Course
            {
                CourseName = name,
                Credits = credits,
                DeptID = deptId
            };

            _repository.Add(course);
        }
        public void RemoveCourse(int id) => _repository.Delete(id);
    }
}