using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using StudentSystem.Core;

namespace StudentSystem.Infrastructure
{
    public class ClubRepository
    {
        private readonly AppDbContext _context;

        public ClubRepository()
        {
            _context = new AppDbContext();
        }

        public List<Club> GetAll()
        {
            var list = new List<Club>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM Clubs", conn))
                {
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

        // YENİ: Bir kulübe üye olan öğrencileri getirir
        public List<Student> GetMembers(int clubId)
        {
            var list = new List<Student>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                // JOIN işlemi ile StudentClubs tablosu üzerinden Öğrencilere ulaşıyoruz
                string query = @"
                    SELECT s.* FROM Students s
                    JOIN StudentClubs sc ON s.StudentID = sc.StudentID
                    WHERE sc.ClubID = @cid";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clubId);
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
                                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : ""
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Add(string name, string desc)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Clubs (ClubName, Description, EstablishmentDate) VALUES (@nm, @dsc, @date)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nm", name);
                    cmd.Parameters.AddWithValue("@dsc", desc);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}