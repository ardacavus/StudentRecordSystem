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
                string query = @"
                    SELECT c.CourseID, c.CourseName, c.Credits, c.DeptID, i.Title, i.FirstName, i.LastName
                    FROM Courses c
                    LEFT JOIN Instructors i ON c.InstructorID = i.InstructorID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string instName = "Unassigned";
                            if (reader["FirstName"] != System.DBNull.Value)
                            {
                                instName = $"{reader["Title"]} {reader["FirstName"]} {reader["LastName"]}";
                            }

                            list.Add(new Course
                            {
                                CourseID = (int)reader["CourseID"],
                                CourseName = reader["CourseName"].ToString(),
                                Credits = (int)reader["Credits"],
                                DeptID = reader["DeptID"] != System.DBNull.Value ? (int)reader["DeptID"] : 0,
                                InstructorName = instName
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

        public void Delete(int id)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmdEnr = new SqlCommand("DELETE FROM Enrollments WHERE CourseID = @id", conn))
                {
                    cmdEnr.Parameters.AddWithValue("@id", id);
                    cmdEnr.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM Courses WHERE CourseID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}