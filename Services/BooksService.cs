using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;

namespace Services
{
    public class BooksService : IBooksService
    {
        // private field
        private readonly BooksDbContext _dbContext;
        private readonly IAuthorsService _authorsService;

        public BooksService(BooksDbContext booksDbContext, IAuthorsService authorsService)
        {
            _dbContext = booksDbContext;
            _authorsService = authorsService;
        }

        private string FormatDate(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);
            string formattedDate = date.ToString("dd MMM yyyy");

            return formattedDate;
        }

        private BookResponse ConvertBookToBookResponse(Book book)
        {
            BookResponse bookRes = book.ToBookResponse();
            bookRes.AuthorName = _authorsService.GetAuthorById(book.AuthorId)?.AuthorName; // <-- Here 
            return bookRes;
        }

        public BookResponse AddBook(BookAddRequest? bookAddRequest)
        {
            // check if null & validation checks
            if (bookAddRequest == null) throw new ArgumentNullException(nameof(bookAddRequest));
            if (string.IsNullOrEmpty(bookAddRequest.BookName)) throw new ArgumentException("Book must have a name");

            // Model validations
            ValidationHelper.ValidateModels(bookAddRequest);

            // Return - convert to book, generate an Id, add new book to list of books and return book as book response
            Book book = bookAddRequest.ToBook();
            book.BookId = new Guid();

            // Add book
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();

            BookResponse bookRes = ConvertBookToBookResponse(book);

            return bookRes; // return ConvertBookToBookResponse(book);
        }

        public List<BookResponse> GetAllBooks()
        {
            return _dbContext.Books.ToList().Select(n => ConvertBookToBookResponse(n)).ToList(); // SELECT * from books
        }

        public BookResponse? GetBookById(Guid? bookId)
        {
            if (bookId == null) return null;

            Book? book = _dbContext.Books.FirstOrDefault(temp => temp.BookId == bookId);
            if (book == null) return null;

            return ConvertBookToBookResponse(book);
        }

        public List<BookResponse> GetFilteredBooks(string searchBy, string? searchString)
        {
            List<BookResponse> allBooks = GetAllBooks();
            List<BookResponse> matchingBooks = allBooks;
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return matchingBooks;

            
            switch (searchBy)
            {
                case nameof(BookResponse.BookName):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.BookName)?
                    x.BookName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(BookResponse.Publisher):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.Publisher) ?
                    x.Publisher.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(BookResponse.Genress): // make serach for more than 1 genre
                    List<BookResponse> filtered = new();
                    foreach (var book in allBooks) if (book.Genress.Contains(searchString)) filtered.Add(book);
                    matchingBooks = filtered;
                    break;
                case nameof(BookResponse.PublishedDate):
                    matchingBooks = allBooks.Where(x => (x.PublishedDate != null) ?
                    x.PublishedDate.Value.ToString("dd MMM yyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(BookResponse.IsOngoing):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.BookName) ?
                    x.BookName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(BookResponse.AuthorId):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.AuthorName) ?
                    x.AuthorName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                default: matchingBooks = allBooks; break;
            }

            return matchingBooks;
        }

        public List<BookResponse> GetSortedBooks(List<BookResponse> allBooks, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy)) return allBooks;

            List<BookResponse> sortedBooks = (sortBy, sortOrder) switch
            {
                // Name
                (nameof(BookResponse.BookName), SortOrderOptions.Asc) => allBooks.OrderBy(x => x.BookName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(BookResponse.BookName), SortOrderOptions.Desc) => allBooks.OrderByDescending(x => x.BookName, StringComparer.OrdinalIgnoreCase).ToList(),

                // Publisher
                (nameof(BookResponse.Publisher), SortOrderOptions.Asc) => allBooks.OrderBy(x => x.Publisher, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(BookResponse.Publisher), SortOrderOptions.Desc) => allBooks.OrderByDescending(x => x.Publisher, StringComparer.OrdinalIgnoreCase).ToList(),

                // Genre
                (nameof(BookResponse.Genre), SortOrderOptions.Asc) => allBooks.OrderBy(x => x.Genre, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(BookResponse.Genre), SortOrderOptions.Desc) => allBooks.OrderByDescending(x => x.Genre, StringComparer.OrdinalIgnoreCase).ToList(),

                // Published Date
                (nameof(BookResponse.PublishedDate), SortOrderOptions.Asc) => allBooks.OrderBy(x => x.PublishedDate).ToList(),
                (nameof(BookResponse.PublishedDate), SortOrderOptions.Desc) => allBooks.OrderByDescending(x => x.PublishedDate).ToList(),

                // Book Age
                (nameof(BookResponse.BookAge), SortOrderOptions.Asc) => allBooks.OrderBy(x => x.BookAge).ToList(),
                (nameof(BookResponse.BookAge), SortOrderOptions.Desc) => allBooks.OrderByDescending(x => x.BookAge).ToList(),

                // Author Name
                (nameof(BookResponse.AuthorName), SortOrderOptions.Asc) => allBooks.OrderBy(x => x.AuthorName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(BookResponse.AuthorName), SortOrderOptions.Desc) => allBooks.OrderByDescending(x => x.AuthorName, StringComparer.OrdinalIgnoreCase).ToList(),

                // Is Ongoing Status
                (nameof(BookResponse.IsOngoing), SortOrderOptions.Asc) => allBooks.OrderBy(x => x.IsOngoing.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(BookResponse.IsOngoing), SortOrderOptions.Desc) => allBooks.OrderByDescending(x => x.IsOngoing.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
            
                _ => allBooks // default case
            };

            return sortedBooks;
        }

        public BookResponse UpdateBook(BookUpdateRequest bookUpdateRequest)
        {
            if (bookUpdateRequest == null) throw new ArgumentNullException(nameof(Book));

            Services.Helpers.ValidationHelper.ValidateModels(bookUpdateRequest);

            Book? book = _dbContext.Books.FirstOrDefault(x => x.BookId == bookUpdateRequest.BookId);
            if (book == null) throw new ArgumentException("Given book doesn't exist");

            // update matching returned book with bookUpdateRequest details | Entitystate.modified
            book.BookName = bookUpdateRequest.BookName;
            book.PublishedDate = bookUpdateRequest.PublishedDate;
            book.BookRating = bookUpdateRequest.BookRating;
            book.Publisher = bookUpdateRequest.Publisher;
            book.AuthorId = bookUpdateRequest.AuthorId;
            book.Genre = bookUpdateRequest.Genre.ToString();
            book.Genres = bookUpdateRequest.Genres;
            book.IsOngoing = bookUpdateRequest.IsOngoing;

            _dbContext.SaveChanges(); // save changes

            return ConvertBookToBookResponse(book);
        }

        public bool DeleteBook(Guid? bookId)
        {
            if (bookId == null) throw new ArgumentNullException(nameof(bookId));

            Book? book = _dbContext.Books.FirstOrDefault(x => x.BookId == bookId); // check if book is valid
            if (book == null) return false;

            _dbContext.Books.Remove(_dbContext.Books.First(c => c.BookId == bookId));
            _dbContext.SaveChanges();

            return true;
        }
    }
}
