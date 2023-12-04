using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base (options){ } // Constructor

        public virtual DbSet<Author>? Authors { get; set; }
        public virtual DbSet<Book>? Books { get; set; }

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

        public List<Book> sp_GetAllBooks()
        {
            // return Books.FromSqlRaw("SELECT BookId, BookName, BookRating, Publisher, PublishedDate, Genre, Genres, AuthorId, IsOngoing FROM [dbo].[Books]").ToList();
            return Books.FromSqlRaw("EXECUTE [dbo].[GetAllBooks]").ToList();
        }

        public int sp_AddBooks(Book book)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@BookId", book.BookId),
                new SqlParameter("@BookRating", book.BookRating),
                new SqlParameter("@BookName", book.BookName),
                new SqlParameter("@Publisher", book.Publisher),
                new SqlParameter("@PublishedDate", book.PublishedDate),
                new SqlParameter("@Genre", book.Genre),
                new SqlParameter("@Genres", book.Genres),
                new SqlParameter("@AuthorId", book.AuthorId),
                new SqlParameter("@IsOngoing", book.IsOngoing)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[AddBooks] @BookId, @BookRating, @BookName, @Publisher, @PublishedDate, @Genre, @Genres, @AuthorId, @IsOngoing", param);
        }
    }
}
