namespace StudentRecordsApp.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Course { get; set; }
        public int YearLevel { get; set; }
    }
}
