using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace xTests
{
    public class BooksServiceTest
    {
        private readonly IBooksService _booksService;
        private readonly IAuthorsService _authorsService;
        private readonly ITestOutputHelper _helper;

        public BooksServiceTest(ITestOutputHelper helper)
        {
            _authorsService = new AuthorsService(new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>().Options));
            _booksService = new BooksService(new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>().Options), _authorsService);
            _helper = helper;
        }

        #region AddBook 
        // Tests for AddBook() method

        [Fact] // Throw null exception if argument is null value
        public void AddBook_NullBook() 
        {
            // Arrange
            BookAddRequest? req = null;

            // Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _booksService.AddBook(req);
            });
        }

        [Fact] // Throw null exception if BookName argument is null value
        public void AddBook_NullBookName()
        {
            // Arrange
            BookAddRequest? req = new() { BookName = null };

            // Act
            Assert.Throws<ArgumentException>(() =>
            {
                _booksService.AddBook(req);
            });
        }

        [Fact] 
        public void AddBook_ValidBookName()
        {
            // Arrange
            BookAddRequest? req = new()
            {
                BookName = "Sword King",
                Publisher = "Asura Scans",
                PublishedDate = DateTime.Parse("2020-01-01"),
                Genre = GenreOptions.Action,
                AuthorId = Guid.NewGuid(),
                IsOngoing = false
            };

            BookResponse bookRes = _booksService.AddBook(req);

            // Act
            Assert.True(bookRes.BookName == req.BookName);
        }

        [Fact] // Add book int book list if all details are properly filled out 
        public void AddBook_CorrectDetails() // -> returns an object of BookResponse with newly generated book id
        {
            // Arrange
            BookAddRequest? req = new()
            {
                BookName = "Solo Leveling",
                Publisher = "Asura Scans",
                PublishedDate = DateTime.Parse("2020-01-01"),
                Genre = GenreOptions.Action,
                AuthorId = Guid.NewGuid(),
                IsOngoing = false
            };

            // Act
            BookResponse bookRes = _booksService.AddBook(req);
            List<BookResponse> listBook = _booksService.GetAllBooks();

            // Assert
            Assert.Contains(bookRes, listBook);
        }

        #endregion

        #region GetBookById
        [Fact] // return null if BookId is null
        public void GetBookById_NullBookId()
        {
            Guid? bookId = null;
            BookResponse? bookRes = _booksService.GetBookById(bookId);

            Assert.Null(bookRes);
        }

        
        [Fact] // return valid book details if BookId is valid
        public void GetBookById_ValidBookId()
        {
            AuthorAddRequest authorReq = new() { AuthorName = "Irene" };
            AuthorResponse authorRes = _authorsService.AddAuthor(authorReq);

            _helper.WriteLine($"authorRes.AuthorName: {authorRes.AuthorName}");

            BookAddRequest? req = new() // bookId will be provided once we call the add book method
            {
                BookName = "Solo Leveling", Publisher = "Asura Scans",
                PublishedDate = DateTime.Parse("2020-01-01"), Genre = GenreOptions.Action,
                AuthorId = authorRes.AuthorId, IsOngoing = false
            };

            BookResponse bookRes = _booksService.AddBook(req); // expected
            _helper.WriteLine($"bookRes.AuthorName: {bookRes.AuthorName}"); 

            BookResponse? bookGet = _booksService.GetBookById(bookRes.BookId); // actual
            _helper.WriteLine($"bookRes.AuthorName: {bookGet.AuthorName}");

            // Assert
            Assert.Equal(bookRes, bookGet);
        }

        #endregion

        #region GetAllBooks
        [Fact] // returns an empty list of book list is empty
        public void GetAllBooks_EmptyBookList()
        {
            List<BookResponse> booksList = _booksService.GetAllBooks();
            Assert.Empty(booksList); // checks if a list is empty
        }

        [Fact]
        public void GetAllBooks_ReturnAddedBooks()
        {
            List<BookAddRequest> bookReq = ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach(var item in bookReq)
            {
                BookResponse bookRes = _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            foreach (var n in booksListAddRes) _helper.WriteLine($"Expected: {n.BookName} - {n.Publisher} - {n.AuthorName}"); // output

            List<BookResponse> list = _booksService.GetAllBooks();
            Assert.NotEmpty(list);

            foreach (var n in list) _helper.WriteLine($"Actual: {n.BookName} - {n.Publisher} - {n.AuthorName}"); // output
            
            foreach (BookResponse bookRes in booksListAddRes)
            {
                Assert.Contains(bookRes, list);
            }
        }
        #endregion

        #region GetFilteredBooks
        [Fact] // if search text is empty - return all values
        public void GetFilteredBooks_EmptySearch()
        {
            List<BookAddRequest> bookReq = ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach (var item in bookReq)
            {
                BookResponse bookRes = _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            foreach (var n in booksListAddRes) _helper.WriteLine($"Expected: {n.BookName} - {n.Publisher} - {n.AuthorName}"); // output

            List<BookResponse> list = _booksService.GetFilteredBooks(nameof(Book.BookName), "");

            foreach (var n in list) _helper.WriteLine($"Actual: {n.BookName} - {n.Publisher} - {n.AuthorName}"); // output

            foreach (BookResponse bookRes in booksListAddRes)
            {
                Assert.Contains(bookRes, list);
            }
        }

        [Fact] // if search text is not empty - return all values based on search property (BookName, Publisher etc)
        public void GetFilteredBooks_SearchByBookName()
        {

            List<BookAddRequest> bookReq = ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach (var item in bookReq)
            {
                BookResponse bookRes = _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            foreach (var n in booksListAddRes) _helper.WriteLine($"Expected: {n.ToString()}"); // output

            List<BookResponse> list = _booksService.GetFilteredBooks(nameof(Book.BookName), "Sword");

            foreach (var n in list) _helper.WriteLine($"Actual: {n.ToString()}"); // output

            foreach (BookResponse bookRes in booksListAddRes)
            {
                if (bookRes != null)
                {
                    if (bookRes.BookName.Contains("sword", StringComparison.OrdinalIgnoreCase)) Assert.Contains(bookRes, list);
                }
                
            }
        }
        #endregion

        #region GetSortedBooks
        [Fact] // sorting by BookName in descending order -> returns books list in descending orders
        public void GetSortedBooks_SearchByBookName()
        {
            List<BookAddRequest> bookReq = ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach (var item in bookReq)
            {
                BookResponse bookRes = _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            List<BookResponse> allBooks = _booksService.GetAllBooks();
            
            // Act
            List<BookResponse> listSortDesc = _booksService.GetSortedBooks(allBooks, nameof(Book.BookName), SortOrderOptions.Desc);

            foreach (var n in listSortDesc) _helper.WriteLine($"Actual: {n.ToString()}"); // output
            booksListAddRes = booksListAddRes.OrderByDescending(x => x.BookName).ToList();

            // Assert
            for(int i = 0; i < booksListAddRes.Count(); i++)
            {
                Assert.Equal(booksListAddRes[i].BookName, listSortDesc[i].BookName);
            }
        }
        #endregion

        #region UpdateBook

        [Fact] // null book check
        public void UpdateBook_NullBook()
        {
            BookUpdateRequest? bookUpdateReq = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _booksService.UpdateBook(bookUpdateReq);
            });

        }

        [Fact] // invalid book Id
        public void UpdateBook_InvalidBookId()
        {
            BookUpdateRequest? bookUpdateReq = new() { BookId = Guid.NewGuid() };

            Assert.Throws<ArgumentException>(() =>
            {
                _booksService.UpdateBook(bookUpdateReq);
            });

        }

        [Fact] // null book name - throw argument exception
        public void UpdateBook_NullBookName()
        {
            // Arrange
            AuthorAddRequest authorAddReq = new() { AuthorName = "Hanni Pham" };
            AuthorResponse authorRes = _authorsService.AddAuthor(authorAddReq); // authorRes will no have the newly generated AuthorId
            
            BookAddRequest bookAddReq = new() { BookName = "How To", AuthorId = authorRes.AuthorId, Publisher = "Asura Scans", IsOngoing = true };
            BookResponse bookRes = _booksService.AddBook(bookAddReq);

            BookUpdateRequest? bookUpdateReq = bookRes.ToBookUpdateRequest();
            bookUpdateReq.BookName = null;

            Assert.Throws<ArgumentException>(() =>
            {
                _booksService.UpdateBook(bookUpdateReq); // Act
            });
        }

        #endregion

        #region DeleteBook

        [Fact] // valid person ID should result in deletion of book - return true fomr DeleteBook() method
        public void DeleteBook_ValidBookId()
        {
            // Arrange
            AuthorAddRequest authorAddReq = new() { AuthorName = "Irene" };
            AuthorResponse authorRes = _authorsService.AddAuthor(authorAddReq);

            BookAddRequest? bookAddReq = new() // bookId will be provided once we call the add book method
            {
                BookName = "Book B", Publisher = "Asura Scans",
                PublishedDate = DateTime.Parse("2020-01-01"),Genre = GenreOptions.Action,
                AuthorId = authorRes.AuthorId, IsOngoing = false
            };

            BookResponse bookRes = _booksService.AddBook(bookAddReq);

            // Assert
            Assert.True(_booksService.DeleteBook(bookRes.BookId));
        }

        [Fact] // valid person ID should result in deletion of book - return true fomr DeleteBook() method
        public void DeleteBook_InvalidBookId()
        {
            // Assert
            Assert.False(_booksService.DeleteBook(Guid.NewGuid()));
        }

        #endregion

        public List<BookAddRequest> ReusableAddBookMethod() // Legacy
        {
            AuthorAddRequest authorReq = new() { AuthorName = "Irene" };
            AuthorResponse authorRes = _authorsService.AddAuthor(authorReq);
            _helper.WriteLine($"authorRes.AuthorName: {authorRes.AuthorName}");

            BookAddRequest? req1 = new() // bookId will be provided once we call the add book method
            {
                BookName = "Book B",
                Publisher = "Asura Scans",
                PublishedDate = DateTime.Parse("2020-01-01"),
                Genre = GenreOptions.Action,
                AuthorId = authorRes.AuthorId,
                IsOngoing = false
            };

            BookAddRequest? req2 = new() // bookId will be provided once we call the add book method
            {
                BookName = "Book A",
                Publisher = "Asura Scans",
                PublishedDate = DateTime.Parse("2020-01-01"),
                Genre = GenreOptions.Action,
                AuthorId = authorRes.AuthorId,
                IsOngoing = false
            };

            List<BookAddRequest> bookReq = new() { req1, req2, };

            return bookReq;
        }
    }
}