using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using StudentSystem.Core;

namespace StudentSystem.Infrastructure
{
    public class StudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository()
        {
            _context = new AppDbContext();
        }

        // Tüm Öğrencileri Listele
        public List<Student> GetAll()
        {
            var list = new List<Student>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Students";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Student
                            {
                                StudentID = (int)reader["StudentID"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                DeptID = reader["DeptID"] != DBNull.Value ? (int)reader["DeptID"] : 0
                            });
                        }
                    }
                }
            }
            return list;
        }

        // Yeni Öğrenci Ekle
        public void Add(Student student)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Students (FirstName, LastName, Email, DeptID) VALUES (@fn, @ln, @em, @did)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fn", student.FirstName);
                    cmd.Parameters.AddWithValue("@ln", student.LastName);
                    cmd.Parameters.AddWithValue("@em", student.Email);
                    cmd.Parameters.AddWithValue("@did", student.DeptID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Öğrenci Sil
        public void Delete(int id)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Students WHERE StudentID = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}