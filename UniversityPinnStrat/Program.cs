using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UniversityPinnStrat
{
    class Program
    {
        public const string Enrolled = "Enrolled";
        static void Main(string[] args)
        {
            InitializeHttpClientAsync();

            Console.WriteLine("Please select an option:");
            Console.WriteLine("1) Extract, Transform, Load Students");
            Console.WriteLine("2) Auto Drop Students");

            bool optionSelected = false;
            while (!optionSelected)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        BL.ETL extractStudents = new BL.ETL();
                        extractStudents.SaveAllStudentsToDatabaseAsync().Wait();
                        optionSelected = true;
                        break;
                    case "2":
                        BL.AutoDrop autoDropStudents = new BL.AutoDrop();
                        autoDropStudents.RunAsync().Wait();
                        optionSelected = true;
                        break;
                    default:
                        Console.WriteLine("Invalid Option, try again:");
                        break;
                }
            }

            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }

        static void InitializeHttpClientAsync()
        {
            client.BaseAddress = new Uri("http://university.pinnstrat.com:8888");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static HttpClient client = new HttpClient(); 
    }
}
