using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions options) : base (options)
        {

        }

        public DbSet<Author>? Authors { get; set; }
        public DbSet<Book>? Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>().ToTable("Authors");
            modelBuilder.Entity<Book>().ToTable("Books");

            // Seeding Data
            string authorsJson = File.ReadAllText("authors.json");
            string booksJson = File.ReadAllText("books.json");

            List<Author>? authors = System.Text.Json.JsonSerializer.Deserialize<List<Author>>(authorsJson);
            List<Book>? books = System.Text.Json.JsonSerializer.Deserialize<List<Book>>(booksJson);

            foreach (var author in authors) modelBuilder.Entity<Author>().HasData(author);
            foreach (var book in books) modelBuilder.Entity<Book>().HasData(book);
        }
    }
}
