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

        // Kullanıcı adı ve şifreyi kontrol eden metod
        public User Login(string username, string password)
        {
            User user = null;

            using (var conn = _context.GetConnection())
            {
                conn.Open();
                // SQL Injection'a karşı parametreli sorgu kullanıyoruz (Clean Code & Güvenlik)
                string query = "SELECT * FROM Users WHERE Username = @u AND PasswordHash = @p";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = (int)reader["UserID"],
                                Username = reader["Username"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }
            return user; // Kullanıcı varsa döner, yoksa null döner
        }
    }
}