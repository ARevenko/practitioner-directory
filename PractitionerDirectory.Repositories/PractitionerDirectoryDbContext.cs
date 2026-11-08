using Microsoft.EntityFrameworkCore;
using PractitionerDirectory.Repositories.Models;

namespace PractitionerDirectory.Repositories
{
    public class PractitionerDirectoryDbContext : DbContext
    {
        public PractitionerDirectoryDbContext(DbContextOptions<PractitionerDirectoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Practitioner> Practitioners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
