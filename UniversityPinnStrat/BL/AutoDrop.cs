using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UniversityPinnStrat.BL
{
    class AutoDrop
    {
        const int minCreditHoursForFullTime = 10;
        const int minGradeToPreventDrop = 40;
        public AutoDrop()
        {
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Running auto drop students... please wait.");
            //Reset application state on start
            await Services.UniversityApi.ResetApiStateAsync(Program.client);
            if (AllStudentsLoaded())
            {
                Console.WriteLine("Processing students to drop...");
                using (var db = new UniversityPinnStratContext())
                {
                    foreach(var student in db.Students)
                    {
                        OutputPercentage(student.StudentId, db.Students.Count());
                        if (student.EnrolledCreditHours >= minCreditHoursForFullTime)
                        {
                            List<int> coursesToDrop = await GetCoursesToDropForAsync(student);
                            foreach(var courseId in coursesToDrop)
                            {
                                DropStudentFromCourse(courseId, student.StudentId);
                            }
                        }
                    }
                }

                await ValidateResultsAsync();
            }
            else
            {
                Console.WriteLine("Please load all Students before starting Auto Drop process.");
            }
        }

        private bool AllStudentsLoaded()
        {
            bool success = false;
            int totalStudents = 250; //Safety precaution to ensure step 1 (ETL) was originally ran (not ideal for production). 
            using (var db = new UniversityPinnStratContext())
            {
                if (db.Students.Count() == totalStudents)
                {
                    success = true;
                }
            }

            return success;
        }

        private async Task<List<int>> GetCoursesToDropForAsync(Student student)
        {
            List<int> courses = new List<int>();
            double currentCreditHours = student.EnrolledCreditHours;

            var studentInfo = await Services.UniversityApi.GetStudentAsync(Program.client, student.StudentId);
            var coursesToDrop = studentInfo.Courses.Where(c => c.Grade < minGradeToPreventDrop && c.Status == Program.Enrolled).OrderBy(c => c.Grade);
            if (coursesToDrop.Any())
            {
                bool allowedToDrop = (currentCreditHours >= minCreditHoursForFullTime);
                foreach(var course in coursesToDrop)
                {
                    var newCreditHours = currentCreditHours - course.CreditHours;
                    allowedToDrop = (newCreditHours >= minCreditHoursForFullTime);
                    if (allowedToDrop)
                    {
                        courses.Add(course.Id);
                        currentCreditHours = newCreditHours;
                    }
                }
            }
            
            return courses;
        }

        private void DropStudentFromCourse(int courseId, int studentId)
        {
            Services.UniversityApi.CourseDropAsync(Program.client, courseId, studentId).Wait();
        }

        private async Task ValidateResultsAsync()
        {
            HttpResponseMessage response = await Services.UniversityApi.Validate(Program.client);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Success: {response.ReasonPhrase}");
            }
            else
            {
                await Services.UniversityApi.ResetApiStateAsync(Program.client);
                Console.WriteLine($"Error: {response.ReasonPhrase} (API has been reset, fix program and run again.)");
            }
        }

        private void OutputPercentage(double current, double total)
        {
            Console.Write("\r{0}% done processing students.", (int)((current / total) * 100));
        }
    }
}
