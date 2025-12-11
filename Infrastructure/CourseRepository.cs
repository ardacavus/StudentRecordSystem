using System.Collections.Generic;
using System.Data.SqlClient;
using StudentSystem.Core;

namespace StudentSystem.Infrastructure
{
    public class CourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository()
        {
            _context = new AppDbContext();
        }

        public List<Course> GetAll()
        {
            var list = new List<Course>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Courses";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Course
                            {
                                CourseID = (int)reader["CourseID"],
                                CourseName = reader["CourseName"].ToString(),
                                Credits = (int)reader["Credits"],
                                DeptID = reader["DeptID"] != System.DBNull.Value ? (int)reader["DeptID"] : 0
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Add(Course course)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Courses (CourseName, Credits, DeptID) VALUES (@name, @cred, @did)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", course.CourseName);
                    cmd.Parameters.AddWithValue("@cred", course.Credits);
                    cmd.Parameters.AddWithValue("@did", course.DeptID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}