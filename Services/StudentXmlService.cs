using System.Xml.Linq;
using StudentRecordsApp.Models;

namespace StudentRecordsApp.Services
{
    public class StudentXmlService
    {
        private readonly string xmlFilePath;

        public StudentXmlService(IWebHostEnvironment env)
        {
            xmlFilePath = Path.Combine(env.ContentRootPath, "Data", "students.xml");

            var dir = Path.GetDirectoryName(xmlFilePath)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(xmlFilePath))
                new XDocument(new XElement("Students")).Save(xmlFilePath);
        }

        public List<Student> GetAll()
        {
            var doc = XDocument.Load(xmlFilePath);
            return doc.Root?.Elements("Student").Select(x => new Student
            {
                StudentId = (int?)x.Element("StudentId") ?? throw new InvalidDataException("Missing StudentId"),
                FirstName = x.Element("FirstName")?.Value ?? throw new InvalidDataException("Missing FirstName"),
                LastName = x.Element("LastName")?.Value ?? throw new InvalidDataException("Missing LastName"),
                Course = x.Element("Course")?.Value ?? throw new InvalidDataException("Missing Course"),
                YearLevel = (int?)x.Element("YearLevel") ?? throw new InvalidDataException("Missing YearLevel")
            }).ToList() ?? new List<Student>();
        }

        public void Add(Student student)
        {
            ValidateStudent(student);

            var doc = XDocument.Load(xmlFilePath);
            var students = doc.Root;

            var maxId = students?.Elements("Student").Max(x => (int?)x.Element("StudentId")) ?? 0;
            student.StudentId = maxId + 1;

            students?.Add(new XElement("Student",
                new XElement("StudentId", student.StudentId),
                new XElement("FirstName", student.FirstName),
                new XElement("LastName", student.LastName),
                new XElement("Course", student.Course),
                new XElement("YearLevel", student.YearLevel)
            ));

            doc.Save(xmlFilePath);
        }

        public void Update(Student student)
        {
            ValidateStudent(student);

            var doc = XDocument.Load(xmlFilePath);
            var existing = doc.Root?.Elements("Student")
                .FirstOrDefault(x => 
                    int.TryParse(x.Element("StudentId")?.Value, out var id) && id == student.StudentId);

            if (existing != null)
            {
                existing.Element("FirstName")?.SetValue(student.FirstName ?? string.Empty);
                existing.Element("LastName")?.SetValue(student.LastName ?? string.Empty);
                existing.Element("Course")?.SetValue(student.Course ?? string.Empty);
                existing.Element("YearLevel")?.SetValue(student.YearLevel);
                doc.Save(xmlFilePath);
            }
        }

        public void Delete(int studentId)
        {
            var doc = XDocument.Load(xmlFilePath);
            var toRemove = doc.Root?.Elements("Student")
                .FirstOrDefault(x => 
                    int.TryParse(x.Element("StudentId")?.Value, out var id) && id == studentId);

            if (toRemove != null)
            {
                toRemove.Remove();
                doc.Save(xmlFilePath);
            }
        }

        private void ValidateStudent(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.FirstName))
                throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(student.LastName))
                throw new ArgumentException("Last name is required.");
            if (string.IsNullOrWhiteSpace(student.Course))
                throw new ArgumentException("Course is required.");
            if (student.YearLevel < 1 || student.YearLevel > 4)
                throw new ArgumentException("Year level must be between 1 and 4.");
        }
    }
}
