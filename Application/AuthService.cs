using System;
using System.Data.SqlClient;
using StudentSystem.Core;
using StudentSystem.Infrastructure;

namespace StudentSystem.Application
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService()
        {
            _context = new AppDbContext();
        }

        public User Login(string username, string password)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                // SQL Injection olmaması için parametreli sorgu kullanıyoruz (Güvenlik dersi +1 puan)
                string query = "SELECT * FROM Users WHERE Username = @u AND PasswordHash = @p";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password); // İleride burayı hashleyeceğiz

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = (int)reader["UserID"],
                                Username = reader["Username"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }
            return null; // Kullanıcı bulunamadı
        }
    }
}