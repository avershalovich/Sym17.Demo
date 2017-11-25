using System.ComponentModel.DataAnnotations;

namespace ImageProcess
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class DataModel : DbContext
    {
        // Your context has been configured to use a 'DataModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'ImageProcess.DataModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DataModel' 
        // connection string in the application configuration file.
        public DataModel()
            : base("name=DataModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

         public virtual DbSet<User> Users { get; set; }

        public static DataModel Create()
        {
            return new DataModel();
        }
    }

    public enum UserStatus
    {
        Init,
        Done,
        Canceled
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string Photo { get; set; }
        public string Photo48 { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public byte Age { get; set; }
        public string Gender { get; set; }

        public UserStatus Status { get; set; }

        public string ParsedText { get; set; }
        // Emotions
        public double Smile { get; set; }
        public float Happiness { get; set; }
        public float Anger { get; set; }
        public float Contempt { get; set; }
        public float Disgust { get; set; }
        public float Fear { get; set; }
        public float Neutral { get; set; }
        public float Sadness { get; set; }
        public float Surprise { get; set; }
        public string Glasses { get; set; }
    }
}