using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Entities;
using Moq;
using ServiceContracts;
using Xunit;
using BooksApp.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;

namespace xTests
{
    public class BooksControllerTest
    {
        private readonly IBooksService _booksService;
        private readonly IAuthorsService _authorsService;

        private readonly Mock<IBooksService> _booksServiceMock;
        private readonly Mock<IAuthorsService> _authorsServiceMock;

        private readonly Fixture _fixture;

        public BooksControllerTest()
        {
            _fixture = new Fixture();

            _booksServiceMock = new Mock<IBooksService>();
            _authorsServiceMock = new Mock<IAuthorsService>();

            _booksService = _booksServiceMock.Object;
            _authorsService = _authorsServiceMock.Object;
        }

        #region Index
        [Fact]
        public async Task IndexReturnValidIndexView() // tests index method in books controller
        {
            // Arrange
            List<BookResponse> bookResponseList = _fixture.Create<List<BookResponse>>();
            BooksController booksController = new(_booksService, _authorsService);

            _booksServiceMock.Setup(temp => temp.GetFilteredBooks(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(bookResponseList);

            _booksServiceMock.Setup(temp => temp.GetSortedBooks(It.IsAny<List<BookResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(bookResponseList);

            // Act
            IActionResult result = await booksController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            
        }
        #endregion

        #region Create
        [Fact]
        public async Task CreateNoModelErrorsReturnsCreateView() // tests create method in books controller
        {
            // Arrange
            BookAddRequest bookAddRequest = _fixture.Create<BookAddRequest>();
            BookResponse bookResponse = _fixture.Create<BookResponse>();

            BooksController booksController = new(_booksService, _authorsService);
            List<AuthorResponse> authors = _fixture.Create<List<AuthorResponse>>();

            _authorsServiceMock.Setup(temp => temp.GetAllAuthors()).ReturnsAsync(authors);
            _booksServiceMock.Setup(temp => temp.AddBook(It.IsAny<BookAddRequest>()));

            // Act
            booksController.ModelState.AddModelError("BookName", "Book name is empty");
            IActionResult result = await booksController.Create(bookAddRequest);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
        }
        #endregion
    }
}
