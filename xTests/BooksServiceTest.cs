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
using EntityFrameworkCoreMock;
using AutoFixture;

namespace xTests
{
    // fix tests - convert to async task and use await
    public class BooksServiceTest
    {
        private readonly IBooksService _booksService;
        private readonly IAuthorsService _authorsService;
        private readonly ITestOutputHelper _helper;
        private readonly IFixture _fixture;

        public BooksServiceTest(ITestOutputHelper helper)
        {
            // AutoFixture for dummy objects
            _fixture = new Fixture();

            List<Author>? authorsInitialData = new();
            List<Book>? booksInitialData = new();
            DbContextMock<ApplicationDbContext> dbContextMock = new(new DbContextOptionsBuilder<ApplicationDbContext>().Options); ;

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Authors, authorsInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Books, booksInitialData);

            // create object of AuthorService
            _authorsService = new AuthorsService(dbContext);
            _booksService = new BooksService(dbContext, _authorsService);
            _helper = helper;
        }

        #region AddBook 
        // Tests for AddBook() method

        [Fact] // Throw null exception if argument is null value
        public async Task AddBook_NullBook() 
        {
            // Arrange
            BookAddRequest? req = null;

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _booksService.AddBook(req);
            });
        }

        [Fact] // Throw null exception if BookName argument is null value
        public async Task AddBook_NullBookName()
        {
            // Arrange
            BookAddRequest? req = _fixture.Build<BookAddRequest>().With(temp => temp.BookName, null as string).Create();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _booksService.AddBook(req);
            });
        }

        [Fact] 
        public async Task AddBook_ValidBookName()
        {
            // Arrange - AutoFixture
            BookAddRequest? req = _fixture.Create<BookAddRequest>();

            BookResponse bookRes = await _booksService.AddBook(req);

            // Act
            Assert.True(bookRes.BookName == req.BookName);
        }

        [Fact] // Add book int book list if all details are properly filled out 
        public async Task AddBook_CorrectDetails() // -> returns an object of BookResponse with newly generated book id
        {
            // Arrange - AutoFixture
            BookAddRequest? req = _fixture.Create<BookAddRequest>();

            // Act
            BookResponse bookRes = await _booksService.AddBook(req);
            List<BookResponse> listBook = await _booksService.GetAllBooks();

            // Assert
            Assert.Contains(bookRes, listBook);
        }

        #endregion

        #region GetBookById
        [Fact] // return null if BookId is null
        public async Task GetBookById_NullBookId()
        {
            Guid? bookId = null;
            BookResponse? bookRes = await _booksService.GetBookById(bookId);

            Assert.Null(bookRes);
        }

        
        [Fact] // return valid book details if BookId is valid
        public async Task GetBookById_ValidBookId()
        {
            AuthorAddRequest authorReq = _fixture.Create<AuthorAddRequest>();
            AuthorResponse authorRes = await _authorsService.AddAuthor(authorReq);

            _helper.WriteLine($"authorRes.AuthorName: {authorRes.AuthorName}");

            BookAddRequest? req = _fixture.Build<BookAddRequest>().With(temp => temp.Publisher, "Yahoo").Create();

            // AddBook creates a BookId
            BookResponse bookRes = await _booksService.AddBook(req); // expected
            _helper.WriteLine($"bookRes.AuthorName: {bookRes.AuthorName}"); 

            BookResponse? bookGet = await _booksService.GetBookById(bookRes.BookId); // actual
            _helper.WriteLine($"bookRes.AuthorName: {bookGet.AuthorName}");

            // Assert
            Assert.Equal(bookRes, bookGet);
        }

        #endregion

        #region GetAllBooks
        [Fact] // returns an empty list of book list is empty
        public async Task GetAllBooks_EmptyBookList()
        {
            List<BookResponse> booksList = await _booksService.GetAllBooks();
            Assert.Empty(booksList); // checks if a list is empty
        }

        [Fact]
        public async Task GetAllBooks_ReturnAddedBooks()
        {
            List<BookAddRequest> bookReq = await ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach(var item in bookReq)
            {
                BookResponse bookRes = await _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            foreach (var n in booksListAddRes) _helper.WriteLine($"Expected: {n.BookName} - {n.Publisher} - {n.AuthorName}"); // output

            List<BookResponse> list = await _booksService.GetAllBooks();
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
        public async Task GetFilteredBooks_EmptySearch()
        {
            List<BookAddRequest> bookReq = await ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach (var item in bookReq)
            {
                BookResponse bookRes = await _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            foreach (var n in booksListAddRes) _helper.WriteLine($"Expected: {n.BookName} - {n.Publisher} - {n.AuthorName}"); // output

            List<BookResponse> list = await _booksService.GetFilteredBooks(nameof(Book.BookName), "");

            foreach (var n in list) _helper.WriteLine($"Actual: {n.BookName} - {n.Publisher} - {n.AuthorName}"); // output

            foreach (BookResponse bookRes in booksListAddRes)
            {
                Assert.Contains(bookRes, list);
            }
        }

        [Fact] // if search text is not empty - return all values based on search property (BookName, Publisher etc)
        public async Task GetFilteredBooks_SearchByBookName()
        {

            List<BookAddRequest> bookReq = await ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach (var item in bookReq)
            {
                BookResponse bookRes = await _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            foreach (var n in booksListAddRes) _helper.WriteLine($"Expected: {n}"); // output

            List<BookResponse> list = await _booksService.GetFilteredBooks(nameof(Book.BookName), "Sword");

            foreach (var n in list) _helper.WriteLine($"Actual: {n}"); // output

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
        public async Task GetSortedBooks_SearchByBookName()
        {
            List<BookAddRequest> bookReq = await ReusableAddBookMethod();
            List<BookResponse> booksListAddRes = new();

            foreach (var item in bookReq)
            {
                BookResponse bookRes = await _booksService.AddBook(item);
                booksListAddRes.Add(bookRes);
            }

            List<BookResponse> allBooks = await _booksService.GetAllBooks();
            
            // Act
            List<BookResponse> listSortDesc = await _booksService.GetSortedBooks(allBooks, nameof(Book.BookName), SortOrderOptions.Desc);

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
        public async Task UpdateBook_NullBook()
        {
            BookUpdateRequest? bookUpdateReq = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _booksService.UpdateBook(bookUpdateReq);
            });

        }

        [Fact] // invalid book Id
        public async Task UpdateBook_InvalidBookId()
        {
            BookUpdateRequest? bookUpdateReq = new() { BookId = Guid.NewGuid() };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _booksService.UpdateBook(bookUpdateReq);
            });

        }

        [Fact] // null book name - throw argument exception
        public async Task UpdateBook_NullBookName()
        {
            // Arrange
            AuthorAddRequest authorAddReq = new() { AuthorName = "Hanni Pham" };
            AuthorResponse authorRes = await _authorsService.AddAuthor(authorAddReq); // authorRes will no have the newly generated AuthorId
            
            BookAddRequest bookAddReq = new() { BookName = "How To", AuthorId = authorRes.AuthorId, Publisher = "Asura Scans", IsOngoing = true };
            BookResponse bookRes = await _booksService.AddBook(bookAddReq);

            BookUpdateRequest? bookUpdateReq = bookRes.ToBookUpdateRequest();
            bookUpdateReq.BookName = null;

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _booksService.UpdateBook(bookUpdateReq); // Act
            });
        }

        #endregion

        #region DeleteBook

        [Fact] // valid person ID should result in deletion of book - return true fomr DeleteBook() method
        public async Task DeleteBook_ValidBookId()
        {
            // Arrange
            AuthorAddRequest authorAddReq = new() { AuthorName = "Irene" };
            AuthorResponse authorRes = await _authorsService.AddAuthor(authorAddReq);

            BookAddRequest? bookAddReq = _fixture.Create<BookAddRequest>();

            BookResponse bookRes = await _booksService.AddBook(bookAddReq);

            // Assert
            Assert.True(await _booksService.DeleteBook(bookRes.BookId));
        }

        [Fact] // valid person ID should result in deletion of book - return true fomr DeleteBook() method
        public async Task DeleteBook_InvalidBookId()
        {
            // Assert
            Assert.False(await _booksService.DeleteBook(Guid.NewGuid()));
        }

        #endregion

        public async Task<List<BookAddRequest>> ReusableAddBookMethod() // Legacy
        {
            AuthorAddRequest authorReq = new() { AuthorName = "Irene" };
            AuthorResponse authorRes = await _authorsService.AddAuthor(authorReq);
            _helper.WriteLine($"authorRes.AuthorName: {authorRes.AuthorName}");

            BookAddRequest? req1 = _fixture.Create<BookAddRequest>();
            BookAddRequest? req2 = _fixture.Create<BookAddRequest>();

            List<BookAddRequest> bookReq = new() { req1, req2, };

            return bookReq;
        }
    }
}