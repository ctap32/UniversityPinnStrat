using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityPinnStrat.BL
{
    /// <summary>
    /// Extract, Transform, Load Students
    /// </summary>
    class ETL
    {
        public ETL()
        {
        }
        
        public async Task<bool> SaveAllStudentsToDatabaseAsync()
        {
            Console.WriteLine("Started extracting students... please wait.");
            bool success = false;

            try
            {
                //Clear out DB to completely start with clean slate
                DeleteAllStudents();
                await Services.UniversityApi.ResetApiStateAsync(Program.client);

                List<Models.Student> students = await Services.UniversityApi.GetStudentsAsync(Program.client);

                using (var dbContext = new UniversityPinnStratContext())
                {
                    dbContext.Students.AddRange(await ProcessStudentsAsync(students));
                    Console.WriteLine("Saving students...");
                    dbContext.SaveChanges();
                }

                success = true;    
            } catch (Exception e)
            {
                //Log error and send error email...'
                Console.WriteLine($"Unknown error during Student ETL process: {e.Message}");
            }

            if (success) { Console.WriteLine("Extracted Students completed successfully!"); }
            return success;
        }

        private void DeleteAllStudents()
        {
            using (var dbContext = new UniversityPinnStratContext())
            {
                dbContext.Students.RemoveRange(dbContext.Students);
                dbContext.SaveChanges();
            }
        }

        private async Task<List<Student>> ProcessStudentsAsync (List<Models.Student> responseStudents)
        {
            Console.WriteLine("Processing Students...");
            List<Student> students = new List<Student>();
            foreach(var responseStudent in responseStudents)
            {
                OutputPercentage(responseStudent.Id, responseStudents.Count());
                Student student = new Student();
                student.StudentId = responseStudent.Id;
                student.Name = responseStudent.Name;
                student.Email = responseStudent.Email;

                var studentInfo = await Services.UniversityApi.GetStudentAsync(Program.client, student.StudentId);
                double totalEnrolledCreditHours = 0.0;
                double totalGradeValue = 0.0; 
                foreach (var studentCourse in studentInfo.Courses)
                {
                    if (studentCourse.Status == Program.Enrolled)
                    {
                        totalEnrolledCreditHours += studentCourse.CreditHours;
                        totalGradeValue += (ConvertGradeToLetterGrade(studentCourse.Grade) * studentCourse.CreditHours);
                    }
                }

                student.EnrolledCreditHours = totalEnrolledCreditHours;
                student.CurrentGPA = CalculateGPA(totalGradeValue, totalEnrolledCreditHours);

                students.Add(student);
            }

            return students;
        }

        private double ConvertGradeToLetterGrade(double grade)
        {
            double gradeDivideByTen = Math.Floor(grade / 10);
            switch((int)gradeDivideByTen)
            {
                case 10:
                case 9:
                    return 4.0;
                case 8: return 3.0;
                case 7: return 2.0;
                case 6: return 1.0;
                default: return 0.0;
            }            
        }

        private double CalculateGPA(double totalGradeValue, double totalCreditHours)
        {
            if (totalCreditHours <= 0) { return 0; } //prevent dividing by zero
            return Math.Round((totalGradeValue / totalCreditHours), 2); 
        }

        private void OutputPercentage(double current, double total)
        {
            Console.Write("\r{0}% done processing students.", (int)((current / total) * 100));
        }
    }
}
