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

        public List<Student> GetAllStudents()
        {
            // Buraya ileride "Eğer yetkisi varsa getir" gibi kurallar eklenebilir
            return _repository.GetAll();
        }

        public void RegisterStudent(string ad, string soyad, string email, int bolumId)
        {
            // Basit validasyon (Clean Code kuralı: Logic burada olur)
            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad))
                throw new System.Exception("Ad ve Soyad boş olamaz!");

            var student = new Student
            {
                FirstName = ad,
                LastName = soyad,
                Email = email,
                DeptID = bolumId
            };

            _repository.Add(student);
        }

        public void RemoveStudent(int id)
        {
            _repository.Delete(id);
        }
    }
}