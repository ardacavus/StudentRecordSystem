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

                // 1. Toplam Öğrenci Sayısı (Efficient SQL: COUNT)
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Students", conn))
                {
                    stats.TotalStudents = (int)cmd.ExecuteScalar();
                }

                // 2. Toplam Ders Sayısı
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Courses", conn))
                {
                    stats.TotalCourses = (int)cmd.ExecuteScalar();
                }

                // 3. Okul Not Ortalaması (Efficient SQL: AVG)
                // ISNULL kullandık ki hiç not yoksa hata vermesin, 0 dönsün.
                using (var cmd = new SqlCommand("SELECT ISNULL(AVG(Grade), 0) FROM Enrollments", conn))
                {
                    object result = cmd.ExecuteScalar();
                    stats.AverageGrade = Convert.ToDouble(result);
                }

                // 4. En Başarılı Öğrenci (Efficient SQL: TOP 1 & ORDER BY)
                // Bu tam olarak hocanın istediği "Efficient Method" örneği!
                string topStudentQuery = @"
                    SELECT TOP 1 s.FirstName + ' ' + s.LastName + ' (' + CAST(e.Grade AS VARCHAR) + ')'
                    FROM Enrollments e
                    JOIN Students s ON e.StudentID = s.StudentID
                    ORDER BY e.Grade DESC";

                using (var cmd = new SqlCommand(topStudentQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    stats.TopStudent = result != null ? result.ToString() : "Yok";
                }
            }
            return stats;
        }
    }
}