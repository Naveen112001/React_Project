using Microsoft.EntityFrameworkCore;

namespace api_backend.Models
{
    
    public class LibDB_context : DbContext
    {
       
        public LibDB_context(DbContextOptions<LibDB_context> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-2Q7DA76\\SQLEXPRESS;Database=LibDB;Trusted_Connection=True;MultipleActiveResultSets=true;");
        }
        public DbSet<Book> Books {get;set;}
        
        public DbSet<Student> Students { get; set; }
        public DbSet<Administrator> Administrators { get; set; }

    }
}
