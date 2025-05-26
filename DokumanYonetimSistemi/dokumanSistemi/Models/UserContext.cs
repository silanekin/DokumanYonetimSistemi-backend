using Microsoft.EntityFrameworkCore;

namespace dokumansistem.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Kullanıcılar için DbSet

        // Diğer gerekli DbSet'ler
    }
}


