using System.Collections.Generic;
using StudentSystem.Core;
using StudentSystem.Infrastructure;

namespace StudentSystem.Application
{
    public class StudentService
    {
        private readonly StudentRepository _repository;

        public StudentService()
        {
            _repository = new StudentRepository();
        }

        public List<Student> GetAllStudents() => _repository.GetAll();

        public List<Club> GetStudentClubs(int studentId) => _repository.GetStudentClubs(studentId);

        public void RegisterStudent(string ad, string soyad, string email, string phone, string gender, System.DateTime birthDate, int bolumId)
        {
            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad))
                throw new System.Exception("First Name and Last Name cannot be empty!");

            var student = new Student
            {
                FirstName = ad,
                LastName = soyad,
                Email = email,
                Phone = phone,
                Gender = gender,
                BirthDate = birthDate,
                DeptID = bolumId
            };

            _repository.Add(student);
        }

        public void RemoveStudent(int id) => _repository.Delete(id);
    }
}