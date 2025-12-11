using System;
using System.Data.SqlClient;

namespace StudentSystem.Infrastructure
{
    public class AppDbContext
    {
        // Kanka burası çok önemli. Kendi bilgisayarında çalışırken
        // Server=. (nokta) genelde çalışır. Çalışmazsa SQL Server adını yazarız.
        private readonly string connectionString = "Server=.;Database=StudentSystemDB;Trusted_Connection=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}