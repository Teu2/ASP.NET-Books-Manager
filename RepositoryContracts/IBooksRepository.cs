using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace RepositoryContracts
{
    public interface IBooksRepository
    {
        Task<Book> AddBook(Book book);

        Task<List<Book>> GetAllBooks();

        Task<Book?> GetBookByBookId(Guid bookId);

        Task<List<Book>> GetFilteredBooks(Expression<Func<Book, bool>> predicate); // predicate LINQ expression to check

        Task<bool> DeleteBookByBookId(Guid bookId);

        Task<Book> UpdateBook(Book book);
    }
}
