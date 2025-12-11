namespace StudentSystem.Core
{
    // Veritabanındaki Users tablosunun aynısı
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin, Student, Teacher
    }
}