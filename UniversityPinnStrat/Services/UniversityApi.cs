using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UniversityPinnStrat.Services
{
    class UniversityApi
    {
        const string studentPath = "/Student";
        const string validatePath = "/Validate";
        const string resetPath = "/Reset";

        public static async Task<List<Models.Student>> GetStudentsAsync(HttpClient client)
        {
            List<Models.Student> students = new List<Models.Student>();
            HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}{studentPath}");
            if (response.IsSuccessStatusCode)
            {
                students = await response.Content.ReadAsAsync<List<Models.Student>>();
            }
            else
            {
                //TODO: Add error handling to log error and send error email, etc....
                Console.WriteLine("Unknown error retrieving all Students.");
            }

            return students;
        }

        public static async Task<Models.Student> GetStudentAsync(HttpClient client, int studentId)
        {
            Models.Student student = new Models.Student();
            HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}{studentPath}/{studentId}");
            if (response.IsSuccessStatusCode)
            {
                student = await response.Content.ReadAsAsync<Models.Student>();
            }
            else
            {
                //TODO: Add error handling to log error and send error email, etc....
                Console.WriteLine($"Unknown error retrieving students: {studentId}");
            }

            return student;
        }

        public static async Task CourseDropAsync(HttpClient client, int courseId, int studentId)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync($"{client.BaseAddress}/Course/{courseId}/Drop/{studentId}", "");
            response.EnsureSuccessStatusCode();
        } 

        public static async Task<HttpResponseMessage> Validate(HttpClient client)
        {
            return await client.GetAsync($"{client.BaseAddress}{validatePath}");
        } 

        public static async Task ResetApiStateAsync(HttpClient client)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync($"{client.BaseAddress}{resetPath}", "");
            response.EnsureSuccessStatusCode();
        }
    }
}
