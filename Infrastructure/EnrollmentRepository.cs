using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using StudentSystem.Core;

namespace StudentSystem.Infrastructure
{
    public class EnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository()
        {
            _context = new AppDbContext();
        }

        // Tüm Kayıtları (Öğrenci Adı ve Ders Adıyla Beraber) Getir
        public List<Enrollment> GetAll()
        {
            var list = new List<Enrollment>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                // JOIN işlemi: ID'ler yerine İsimleri çekiyoruz
                string query = @"
                    SELECT 
                        e.EnrollmentID, e.Grade, e.EnrollmentDate,
                        s.FirstName, s.LastName,
                        c.CourseName
                    FROM Enrollments e
                    JOIN Students s ON e.StudentID = s.StudentID
                    JOIN Courses c ON e.CourseID = c.CourseID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Enrollment
                            {
                                EnrollmentID = (int)reader["EnrollmentID"],
                                Grade = (double)reader["Grade"],
                                EnrollmentDate = (DateTime)reader["EnrollmentDate"],
                                // Ekranda göstermek için birleştirdik
                                StudentName = reader["FirstName"] + " " + reader["LastName"],
                                CourseName = reader["CourseName"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // Yeni Ders Kaydı ve Not Girişi
        public void Add(int studentId, int courseId, double grade)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Enrollments (StudentID, CourseID, Grade) VALUES (@sid, @cid, @grade)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@cid", courseId);
                    cmd.Parameters.AddWithValue("@grade", grade);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateGrade(int enrollmentId, double newGrade)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Enrollments SET Grade = @g WHERE EnrollmentID = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@g", newGrade);
                    cmd.Parameters.AddWithValue("@id", enrollmentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}