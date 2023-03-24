using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        public Course() 
        {
            this.Resources = new HashSet<Resource>();
            this.StudentCourses = new HashSet<StudentCourse>();
            this.Homeworks= new HashSet<Homework>();
        }

        public int CourseId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }
       
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public ICollection<Resource> Resources { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<Homework> Homeworks { get; set; }
    }
}
