using DokumanSistem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DokumanSistem.Data
{
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) { }

       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("UserConnection", options =>
                {
                    options.MigrationsAssembly("DokumanSistem.Data");
                });
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentIndex> DocumentIndexes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserCategoryLink> UserCategoryLinks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedDate)
                .ValueGeneratedOnUpdate();

            modelBuilder.Entity<DocumentIndex>()
                .HasOne(di => di.Document)
                .WithMany(d => d.DocumentIndexes)
                .HasForeignKey(di => di.DocumentId);

          
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId);

            modelBuilder.Entity<UserCategoryLink>()
                .HasKey(uc => new { uc.UserId, uc.CategoryId });

        }
    }
}
