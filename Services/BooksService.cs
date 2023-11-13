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
        private readonly List<Book> _books;
        private readonly IAuthorsService _authorsService;

        public BooksService(bool init = true)
        {
            _books = new List<Book>();
            _authorsService = new AuthorsService();

            if (init)
            {
                _books.AddRange(new List<Book>()
                {
                    new Book()
                    {
                        BookId = Guid.Parse("1a895104-1e8f-4c05-9cf0-c09a3adcd3a9"),
                        BookName = "Book A", BookRating = 4, Publisher = "Feedbug",
                        PublishedDate = DateTime.Parse("2023-07-12"), Genres = "Action, Comedy", // $"{Enums.GenreOptions.Adventure}, {Enums.GenreOptions.Comedy}"
                        IsOngoing = false, AuthorId = Guid.Parse("6CB6A381-C136-4AC8-8476-0FCB20F06F2B")
                    },

                    new Book()
                    {
                        BookId = Guid.Parse("d798fc05-fe54-4a2e-ae82-9955a2dc0a56"),
                        BookName = "Book B", BookRating = 3, Publisher = "OTK",
                        PublishedDate = DateTime.Parse("2023-04-08"), Genres = "Drama, Comedy, Adventure, Horror",
                        IsOngoing = true, AuthorId = Guid.Parse("7275844C-6D85-41AE-9292-9F4C66197492")
                    },

                    new Book()
                    {
                        BookId = Guid.Parse("1aab8de6-efd1-4281-8a7b-56eb7340d062"),
                        BookName = "Book C", BookRating = 5, Publisher = "Asura Scans",
                        PublishedDate = DateTime.Parse("2023-04-09"), Genres = "Horror",
                        IsOngoing = false, AuthorId = Guid.Parse("976CD651-EA40-4138-A68A-9EFE92EB6337")
                    },

                    new Book()
                    {
                        BookId = Guid.Parse("47c1d1e3-0fde-4b04-8ee6-219ca83e19cd"),
                        BookName = "Book D", BookRating = 5, Publisher = "Riot Games",
                        PublishedDate = DateTime.Parse("2023-04-10"), Genres = "One Shot",
                        IsOngoing = true, AuthorId = Guid.Parse("48B6490A-4DDB-4606-99E4-F97641FBDCC7")
                    },

                    new Book()
                    {
                        BookId = Guid.Parse("1df9ec41-ff95-46e9-b1b8-b445f9cde561"),
                        BookName = "Book E", BookRating = 2, Publisher = "Bipo Translations",
                        PublishedDate = DateTime.Parse("2023-04-11"), Genres = "Historical, Fantasy",
                        IsOngoing = true, AuthorId = Guid.Parse("0E24B3F8-2167-43C7-8648-7442BA71E15A")
                    },

                }); ;
            }
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
            _books.Add(book);

            BookResponse bookRes = ConvertBookToBookResponse(book);

            return bookRes; // return ConvertBookToBookResponse(book);
        }

        public List<BookResponse> GetAllBooks()
        {
            //List<BookResponse> books = new();

            //foreach(var n in _books)
            //{
            //    books.Add(ConvertBookToBookResponse(n));
            //}

            return _books.Select(n => ConvertBookToBookResponse(n)).ToList();
        }

        public BookResponse? GetBookById(Guid? bookId)
        {
            if(bookId == null) return null;
            Book? book;

            foreach(var currentBook in _books)
            {
                if (currentBook.BookId == bookId)
                {
                    book = currentBook;
                    if (book == null) return null;
                    return ConvertBookToBookResponse(book);  // book.ToBookResponse();
                }
            }

            //Book? book = _books.FirstOrDefault(temp => temp.BookId == bookId);
            //if (book == null) return null;

            return null;
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

            Book? book = _books.FirstOrDefault(x => x.BookId == bookUpdateRequest.BookId);
            if (book == null) throw new ArgumentException("Given book doesn't exist");

            // update matching returned book with bookUpdateRequest details
            book.BookName = bookUpdateRequest.BookName;
            book.PublishedDate = bookUpdateRequest.PublishedDate;
            book.BookRating = bookUpdateRequest.BookRating;
            book.Publisher = bookUpdateRequest.Publisher;
            book.AuthorId = bookUpdateRequest.AuthorId;
            book.Genre = bookUpdateRequest.Genre.ToString();
            book.Genres = bookUpdateRequest.Genres;
            book.IsOngoing = bookUpdateRequest.IsOngoing;

            return ConvertBookToBookResponse(book);
        }

        public bool DeleteBook(Guid? bookId)
        {
            if (bookId == null) throw new ArgumentNullException(nameof(bookId));

            Book? book = _books.FirstOrDefault(x => x.BookId == bookId);
            if (book == null) return false;

            _books.RemoveAll(x => x.BookId == bookId);

            return true;
        }
    }
}
