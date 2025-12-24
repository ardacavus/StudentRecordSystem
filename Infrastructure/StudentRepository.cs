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
                                DeptID = reader["DeptID"] != DBNull.Value ? (int)reader["DeptID"] : 0,
                                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : "",
                                Gender = reader["Gender"] != DBNull.Value ? reader["Gender"].ToString() : "",
                                BirthDate = reader["BirthDate"] != DBNull.Value ? (DateTime?)reader["BirthDate"] : null
                            });
                        }
                    }
                }
            }
            return list;
        }

        // YENİ: Öğrencinin üye olduğu kulüpleri getirir
        public List<Club> GetStudentClubs(int studentId)
        {
            var list = new List<Club>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT c.* FROM Clubs c
                    JOIN StudentClubs sc ON c.ClubID = sc.ClubID
                    WHERE sc.StudentID = @sid";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Club
                            {
                                ClubID = (int)reader["ClubID"],
                                ClubName = reader["ClubName"].ToString(),
                                Description = reader["Description"].ToString(),
                                EstablishmentDate = reader["EstablishmentDate"] != DBNull.Value ? (DateTime)reader["EstablishmentDate"] : DateTime.MinValue
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Add(Student student)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Students (FirstName, LastName, Email, DeptID, Phone, Gender, BirthDate) VALUES (@fn, @ln, @em, @did, @ph, @gen, @bd)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fn", student.FirstName);
                    cmd.Parameters.AddWithValue("@ln", student.LastName);
                    cmd.Parameters.AddWithValue("@em", student.Email);
                    cmd.Parameters.AddWithValue("@did", student.DeptID);
                    cmd.Parameters.AddWithValue("@ph", (object)student.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@gen", (object)student.Gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@bd", (object)student.BirthDate ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmdEnr = new SqlCommand("DELETE FROM Enrollments WHERE StudentID = @id", conn))
                {
                    cmdEnr.Parameters.AddWithValue("@id", id);
                    cmdEnr.ExecuteNonQuery();
                }
                using (var cmdClub = new SqlCommand("DELETE FROM StudentClubs WHERE StudentID = @id", conn))
                {
                    cmdClub.Parameters.AddWithValue("@id", id);
                    cmdClub.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM Students WHERE StudentID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}