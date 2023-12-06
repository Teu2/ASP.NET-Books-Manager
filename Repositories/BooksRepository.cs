using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BooksRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Book> AddBook(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            return book;
        }

        public async Task<bool> DeleteBookByBookId(Guid bookId)
        {
            _dbContext.RemoveRange(_dbContext.Books.Where(temp => temp.BookId == bookId));
            int deletedRows = await _dbContext.SaveChangesAsync();

            return deletedRows > 0;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _dbContext.Books.Include("Author").ToListAsync();
        }

        public async Task<Book?> GetBookByBookId(Guid bookId)
        {
            return await _dbContext.Books.Include("Author")
                .Where(temp => temp.BookId == bookId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Book>> GetFilteredBooks(Expression<Func<Book, bool>> predicate)
        {
            return await _dbContext.Books.Include("Author")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Book> UpdateBook(Book book)
        {
            Book matchingBook = await _dbContext.Books.FirstOrDefaultAsync(temp => temp.BookId == book.BookId);
            if (matchingBook == null) return null;

            matchingBook.BookName = book.BookName;
            matchingBook.BookRating = book.BookRating;
            matchingBook.PublishedDate = book.PublishedDate;
            matchingBook.Publisher = book.Publisher;
            matchingBook.IsOngoing = book.IsOngoing;
            matchingBook.Author = book.Author;
            matchingBook.Genre = book.Genre;
            matchingBook.Genres = book.Genres;

            await _dbContext.SaveChangesAsync();

            return matchingBook;
        }
    }
}