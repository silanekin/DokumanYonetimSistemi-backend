using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokumanSistem.Data
{
  public  class DocumentDbContextFactory : IDesignTimeDbContextFactory<DocumentDbContext>
    {
        public DocumentDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DocumentDbContext>();
            optionsBuilder.UseSqlServer("server=. ; database=DokumanSistem; Trusted_Connection=true; TrustServerCertificate=True; MultipleActiveResultSets=true");
            return new DocumentDbContext(optionsBuilder.Options);
        }
    }
}
