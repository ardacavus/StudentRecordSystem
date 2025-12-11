using System;
using System.Data.SqlClient;

namespace StudentSystem.Infrastructure
{
    public class AppDbContext
    {
        // Kendi bilgisayarın olduğu için Server=. (nokta) yeterli.
        // Eğer hata alırsan nokta yerine bilgisayar adını yazarız.
        private readonly string connectionString = "Server=.;Database=StudentSystemDB;Trusted_Connection=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}