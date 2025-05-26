using Microsoft.EntityFrameworkCore;


namespace DokumanYonetimSistemi.DataModels
{
    public class SystemAdminContext : DbContext
    {
        public SystemAdminContext(DbContextOptions<SystemAdminContext> options) : base(options) 
        {
            
        }

        public DbSet<User> User { get; set; }
    }
}
