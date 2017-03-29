namespace UniversityPinnStrat
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class UniversityPinnStratContext : DbContext
    {
        // Your context has been configured to use a 'UniversityPinnStratModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'UniversityPinnStrat.UniversityPinnStratModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'UniversityPinnStratModel' 
        // connection string in the application configuration file.
        public UniversityPinnStratContext()
            : base("name=UniversityPinnStratModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<Student> Students { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    public class Student
    {
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public double EnrolledCreditHours { get; set; }
        public double CurrentGPA { get; set; }
    }
}