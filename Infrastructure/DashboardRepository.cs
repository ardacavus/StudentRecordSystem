using System;
using System.Data.SqlClient;
using StudentSystem.Core;

namespace StudentSystem.Infrastructure
{
    public class DashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository()
        {
            _context = new AppDbContext();
        }

        public DashboardStats GetStats()
        {
            var stats = new DashboardStats();

            using (var conn = _context.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Students", conn))
                {
                    stats.TotalStudents = (int)cmd.ExecuteScalar();
                }

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Courses", conn))
                {
                    stats.TotalCourses = (int)cmd.ExecuteScalar();
                }

                
                using (var cmd = new SqlCommand("SELECT ISNULL(AVG(Grade), 0) FROM Enrollments", conn))
                {
                    object result = cmd.ExecuteScalar();
                    stats.AverageGrade = Convert.ToDouble(result);
                }

                
                string topStudentQuery = @"
                    SELECT TOP 1 s.FirstName + ' ' + s.LastName + ' (' + CAST(e.Grade AS VARCHAR) + ')'
                    FROM Enrollments e
                    JOIN Students s ON e.StudentID = s.StudentID
                    ORDER BY e.Grade DESC";

                using (var cmd = new SqlCommand(topStudentQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    stats.TopStudent = result != null ? result.ToString() : "N/A";
                }
            }
            return stats;
        }
    }
}