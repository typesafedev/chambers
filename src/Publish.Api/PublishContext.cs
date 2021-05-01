using Microsoft.EntityFrameworkCore;
using Publish.Core.Entities.DocumentAggregate;

namespace Publish.Api
{
    public class PublishContext : DbContext
    {
        public PublishContext(DbContextOptions<PublishContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Document>().ToTable("Document");
        //}

        // The following configures EF to create a Sqlite database file as `C:\temp\publish.db`.
        // For Mac or Linux, change this to `/tmp/publish.db` or any other absolute path.
        //@"Data Source=C:\temp\publish.db"
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite(@"Data Source=c:\temp\publish.db");
    }
}
