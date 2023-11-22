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
        Task<BookResponse> AddBook(BookAddRequest? bookAddRequest); // add book into list of books

        Task<List<BookResponse>> GetAllBooks(); // retrieves all books

        Task<BookResponse?> GetBookById(Guid? bookId);

        Task<List<BookResponse>> GetFilteredBooks(string searchBy, string? searchString);

        Task<List<BookResponse>> GetSortedBooks(List<BookResponse> allBooks, string sortBy, SortOrderOptions sortOrder);

        Task<BookResponse> UpdateBook(BookUpdateRequest bookUpdateRequest);

        public Task<bool> DeleteBook(Guid? bookId);

        Task<MemoryStream> GetBooksCSV();

        Task<MemoryStream> GetBooksExcel(); // returns memory stream with excel data
    }
}
