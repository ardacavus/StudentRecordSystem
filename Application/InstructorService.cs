using System.Collections.Generic;
using StudentSystem.Core;
using StudentSystem.Infrastructure;

namespace StudentSystem.Application
{
    public class InstructorService
    {
        private readonly InstructorRepository _repository;

        public InstructorService()
        {
            _repository = new InstructorRepository();
        }

        public List<Instructor> GetAllInstructors() => _repository.GetAll();

        public void AddInstructor(string firstName, string lastName, string title, string phone, int deptId)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new System.Exception("First Name and Last Name cannot be empty!");

            var instructor = new Instructor
            {
                FirstName = firstName,
                LastName = lastName,
                Title = title,
                Phone = phone,
                DeptID = deptId
            };

            _repository.Add(instructor);
        }
    }
}