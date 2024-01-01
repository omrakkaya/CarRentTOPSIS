using Microsoft.EntityFrameworkCore;
using RentTOPSIS.Models;
using RentTOPSIS.Models.ViewModels;

namespace RentTOPSIS.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        
        }

        public DbSet<Arabalar> Arabalar { get; set;} 

    }
}
