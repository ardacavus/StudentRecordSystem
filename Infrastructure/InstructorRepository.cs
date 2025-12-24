using System.Collections.Generic;
using System.Data.SqlClient;
using StudentSystem.Core;

namespace StudentSystem.Infrastructure
{
    public class InstructorRepository
    {
        private readonly AppDbContext _context;

        public InstructorRepository()
        {
            _context = new AppDbContext();
        }

        public List<Instructor> GetAll()
        {
            var list = new List<Instructor>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM Instructors", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Instructor
                            {
                                InstructorID = (int)reader["InstructorID"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Title = reader["Title"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                DeptID = (int)reader["DeptID"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Add(Instructor instructor)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Instructors (FirstName, LastName, Title, Phone, DeptID) VALUES (@fn, @ln, @tit, @ph, @did)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fn", instructor.FirstName);
                    cmd.Parameters.AddWithValue("@ln", instructor.LastName);
                    cmd.Parameters.AddWithValue("@tit", instructor.Title);
                    cmd.Parameters.AddWithValue("@ph", instructor.Phone);
                    cmd.Parameters.AddWithValue("@did", instructor.DeptID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}