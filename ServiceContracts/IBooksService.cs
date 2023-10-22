using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Entities;

namespace ServiceContracts
{
    /// <summary>
    /// Business logic for manipulating book entity
    /// </summary>
    public interface IBooksService
    {
        BookResponse AddBook(BookAddRequest? bookAddRequest); // add book into list of books

        List<BookResponse> GetAllBooks(); // retrieves all books

        BookResponse? GetBookById(Guid? bookId);

        /// <summary>
        /// Returns a list of books by filter
        /// </summary>
        /// <param name="searchBy">property to search</param>
        /// <param name="searchString">actual value to search</param>
        /// <returns></returns>
        List<BookResponse> GetFilteredBooks(string searchBy, string? searchString);

        List<BookResponse> GetSortedBooks(List<BookResponse> allBooks, string sortBy, SortOrderOptions sortOrder);

        BookResponse UpdateBook(BookUpdateRequest bookUpdateRequest);

        public bool DeleteBook(Guid? bookId);
    }
}
