using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityPinnStrat.Models
{
    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Course> Courses { get; set; }
    }

    class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double CreditHours { get; set; }
        public string Status { get; set; }
        public double Grade { get; set; }
    }
}
