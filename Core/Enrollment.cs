using System;

namespace StudentSystem.Core
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public double Grade { get; set; } // Not ondalıklı olabilir (75.5 gibi)
        public DateTime EnrollmentDate { get; set; }

        // Ekranda göstermek için Ekstra Özellikler (Veritabanında yok ama işimize yarar)
        public string StudentName { get; set; } // "Ahmet Yılmaz"
        public string CourseName { get; set; }  // "Veritabanı"
    }
}