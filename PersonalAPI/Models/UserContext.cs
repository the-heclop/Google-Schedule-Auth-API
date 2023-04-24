using Microsoft.EntityFrameworkCore;

namespace PersonalAPI.Models
{
    
    public class UserContext : DbContext
    {

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<LoginModel> TokenUsers { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{            
        //    optionsBuilder.UseSqlServer("DefaultConnection");
        //}


    }
    
}
