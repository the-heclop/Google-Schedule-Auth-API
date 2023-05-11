using Microsoft.EntityFrameworkCore;
using PersonalAPI.DTOs;
using PersonalAPI.Models;

namespace PersonalAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySQL("DefaultConnection");
        //}

        public DbSet<LoginModel> TokenUsers { get; set; }

        public DbSet<ShipmentModel> tracking_data { get; set; }

    }
}
