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

        public List<Enrollment> GetAllEnrollments()
        {
            return _repository.GetAll();
        }

        public void AssignGrade(int studentId, int courseId, double grade)
        {
            // Basit kontrol: Not 0-100 arası olmalı
            if (grade < 0 || grade > 100)
                throw new System.Exception("Not 0 ile 100 arasında olmalıdır!");

            _repository.Add(studentId, courseId, grade);
        }
    }
}