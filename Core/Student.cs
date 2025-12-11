using System;

namespace StudentSystem.Core
{
    public class Student
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int DeptID { get; set; } // Hangi bölümde?

        // Ekranda göstermek için yardımcı özellikler (Opsiyonel ama işe yarar)
        public string FullName => $"{FirstName} {LastName}";
    }
}