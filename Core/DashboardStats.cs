namespace StudentSystem.Core
{
    public class DashboardStats
    {
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public double AverageGrade { get; set; }
        public string TopStudent { get; set; } // Örn: "Ahmet Yılmaz (98)"
    }
}