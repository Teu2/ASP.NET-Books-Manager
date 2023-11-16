using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace xTests
{
    public class AuthorsServiceTest
    {
        private readonly IAuthorsService _authorsService;

        public AuthorsServiceTest()
        {
            // create object of AuthorService
            _authorsService = new AuthorsService(new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>().Options));
        }

        #region AddAuthor
        [Fact]
        public void AddAuthor_NullAuthor() // check for null and throw null exception
        {
            // Arrange
            AuthorAddRequest? req = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _authorsService.AddAuthor(req); // Act
            });
        }

        [Fact]
        public void AddAuthor_AuthorNameIsNull() // when AuthorName is null. throw argument exception
        {
            // Arrange
            AuthorAddRequest? req = new AuthorAddRequest() { AuthorName = null };

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _authorsService.AddAuthor(req); // Act
            });
        }

        [Fact]
        public void AddAuthor_AuthorIsDuplicate() // When duplicate, throw another arg exception
        {
            // Arrange
            AuthorAddRequest? req1 = new AuthorAddRequest() { AuthorName = "Ryujin" };
            AuthorAddRequest? req2 = new AuthorAddRequest() { AuthorName = "Ryujin" };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _authorsService.AddAuthor(req1);
                _authorsService.AddAuthor(req2);
            });
        }

        [Fact]
        public void AddAuthor_ProperAuthorNames() // When a proper Author name is supplied, it should add to the list of Authors
        {
            // Arrange
            AuthorAddRequest? req1 = new AuthorAddRequest() { AuthorName = "Irene" };

            // Act
            AuthorResponse authorRes = _authorsService.AddAuthor(req1);
            List<AuthorResponse> authorsFromGetAllAuthors = _authorsService.GetAllAuthors();

            // Assert -  Check if newly added author has actually been added to the list of authors
            Assert.True(authorRes.AuthorId != Guid.Empty);
            Assert.Contains(authorRes, authorsFromGetAllAuthors); // Contains() autmatically calls the Equals() method
        }
        #endregion

        #region GetAllAuthors
        [Fact]
        public void GetAllAuthors_EmptyList()
        {
            // Act
            List<AuthorResponse> authorResList = _authorsService.GetAllAuthors();

            // Assert
            Assert.Empty(authorResList);
        }

        [Fact]
        public void GetAllAuthors_AddSomeAuthors()
        {
            // Arrange
            List<AuthorAddRequest> authorReqList = new()
            {
                new AuthorAddRequest() { AuthorName = "Toriyama" },
                new AuthorAddRequest() { AuthorName = "Akira" },
                new AuthorAddRequest() { AuthorName = "Soon" },
            };

            AuthorAddRequest newAuthor = new AuthorAddRequest() { AuthorName = "Sayama" };
            authorReqList.Add(newAuthor);
            
            // Act
            List<AuthorResponse> testAuthorList = new();
            foreach(var author in authorReqList)
            {
                testAuthorList.Add(_authorsService.AddAuthor(author));
                //_authorsService.AddAuthor(author);
            }

            List<AuthorResponse> actualAuthorResList = _authorsService.GetAllAuthors();
            //testAuthorList = _authorsService.GetAllAuthors();

            foreach(var expectedAuthor in testAuthorList)
            {
                Assert.Contains(expectedAuthor, actualAuthorResList);
            }
            
        }
        #endregion

        #region GetAuthorsById
        [Fact]
        public void GetAuthorById_NullId()
        {
            // Arrange
            Guid? AuthorId = null;

            // Act
            AuthorResponse? res = _authorsService.GetAuthorById(AuthorId);

            // Assert
            Assert.Null(res);
        }

        [Fact]
        public void GetAuthorById_ValidId()
        {
            // Arrange
            AuthorAddRequest addReq = new AuthorAddRequest() { AuthorName = "Jihyo" };
            AuthorResponse addRes = _authorsService.AddAuthor(addReq);

            // Act
            AuthorResponse? getRes = _authorsService.GetAuthorById(addRes.AuthorId);

            // Assert
            Assert.Equal(addRes, getRes);
        }
        #endregion
    }
}
