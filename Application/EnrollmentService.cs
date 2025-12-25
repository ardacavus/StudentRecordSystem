using System.Collections.Generic;
using StudentSystem.Core;
using StudentSystem.Infrastructure;

namespace StudentSystem.Application
{
    public class EnrollmentService
    {
        private readonly EnrollmentRepository _repository;

        public EnrollmentService()
        {
            _repository = new EnrollmentRepository();
        }

        public List<Enrollment> GetAllEnrollments() => _repository.GetAll();
        public List<Enrollment> GetStudentHistory(int studentId) => _repository.GetByStudentId(studentId);

        public void AssignGrade(int studentId, int courseId, double grade)
        {
            if (grade < 0 || grade > 100)
                throw new System.Exception("Grade must be between 0 and 100!");

            _repository.Add(studentId, courseId, grade);
        }
        public void UpdateGrade(int enrollmentId, double newGrade)
        {
            if (newGrade < 0 || newGrade > 100)
                throw new System.Exception("Grade must be between 0 and 100!");
            _repository.UpdateGrade(enrollmentId, newGrade);
        }
    }
}