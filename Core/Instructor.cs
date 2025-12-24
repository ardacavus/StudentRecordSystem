namespace StudentSystem.Core
{
    public class Instructor
    {
        public int InstructorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; } // Prof. Dr. vb.
        public string Phone { get; set; }
        public int DeptID { get; set; }

        public string FullName => $"{Title} {FirstName} {LastName}";
    }
}