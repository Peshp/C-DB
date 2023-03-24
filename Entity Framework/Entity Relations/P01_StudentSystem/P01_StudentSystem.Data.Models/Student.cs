using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student() 
        { 
            this.StudentCourses = new HashSet<StudentCourse>();
            this.Homeworks= new HashSet<Homework>();
        }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<Homework> Homeworks { get; set; }
    }
}