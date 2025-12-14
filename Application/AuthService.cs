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

        // Kullanıcı adı ve şifreyi kontrol eden metod (Stored Procedure kullanır)
        public User Login(string username, string password)
        {
            User user = null;

            using (var conn = _context.GetConnection())
            {
                conn.Open();

                // Düz SQL yerine Stored Procedure adını veriyoruz
                // "sp_LoginUser" senin veritabanında oluşturduğumuz prosedürün adı
                using (var cmd = new SqlCommand("sp_LoginUser", conn))
                {
                    // Komut tipini StoredProcedure olarak belirtmek ZORUNLU
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

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