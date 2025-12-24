using System;

namespace StudentSystem.Core
{
    public class Student
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // --- YENİ EKLENEN ALANLAR ---
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        // ----------------------------

        public int DeptID { get; set; } // Hangi bölümde?

        public string FullName => $"{FirstName} {LastName}";
    }
}