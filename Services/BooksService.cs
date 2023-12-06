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
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services
{
    public class BooksService : IBooksService
    {
        // private field
        private readonly IBooksRepository _BooksRepository;

        public BooksService(IBooksRepository BooksRepository, IAuthorsService authorsService)
        {
            _BooksRepository = BooksRepository;
        }

        private string FormatDate(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);
            string formattedDate = date.ToString("dd MMM yyyy");

            return formattedDate;
        }

        public async Task<BookResponse> AddBook(BookAddRequest? bookAddRequest)
        {
            // check if null & validation checks
            if (bookAddRequest == null) throw new ArgumentNullException(nameof(bookAddRequest));
            if (string.IsNullOrEmpty(bookAddRequest.BookName)) throw new ArgumentException("Book must have a name");

            // model validations
            ValidationHelper.ValidateModels(bookAddRequest);

            // return - convert to book, generate an Id, add new book to list of books and return book as book response
            Book book = bookAddRequest.ToBook();
            book.BookId = Guid.NewGuid();

            // Add book | not stored procedure
            await _BooksRepository.AddBook(book);

            // Add book | stored procedure
            // _dbContext.sp_AddBooks(book);

            BookResponse bookRes = book.ToBookResponse();

            return bookRes; // return ConvertBookToBookResponse(book);
        }

        public async Task<List<BookResponse>> GetAllBooks()
        {
            var books = await _BooksRepository.GetAllBooks();
            return books.Select(x => x.ToBookResponse()).ToList();
            //return _dbContext.sp_GetAllBooks().Select(x => ConvertBookToBookResponse(x)).ToList();
            //return _dbContext.Books.ToList().Select(n => ConvertBookToBookResponse(n)).ToList(); // SELECT * from books
        }

        public async Task<BookResponse?> GetBookById(Guid? bookId)
        {
            if (bookId == null) return null;

            Book? book = await _BooksRepository.GetBookByBookId(bookId.Value);
            if (book == null) return null;
            
            return book.ToBookResponse();
        }

        public async Task<List<BookResponse>> GetFilteredBooks(string searchBy, string? searchString)
        {
            List<BookResponse> allBooks = await GetAllBooks();
            List<BookResponse> matchingBooks = allBooks;
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return matchingBooks;
            
            switch (searchBy)
            {
                case nameof(BookResponse.BookName):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.BookName)?
                    x.BookName.Contains(searchString) : true)).ToList(); break;
                case nameof(BookResponse.Publisher):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.Publisher) ?
                    x.Publisher.Contains(searchString) : true)).ToList(); break;
                case nameof(BookResponse.Genress): // make search for more than 1 genre
                    List<BookResponse> filtered = new();
                    foreach (var book in allBooks)
                    {
                        foreach(var genre in book.Genress) if(genre.Contains(searchString)) filtered.Add(book);
                    }
                    matchingBooks = filtered; 
                    break;
                case nameof(BookResponse.PublishedDate):
                    matchingBooks = allBooks.Where(x => (x.PublishedDate != null) ?
                    x.PublishedDate.Value.ToString("dd MMM yyy").Contains(searchString) : true).ToList(); break;
                case nameof(BookResponse.IsOngoing):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.BookName) ?
                    x.BookName.Contains(searchString) : true)).ToList(); break;
                case nameof(BookResponse.AuthorId):
                    matchingBooks = allBooks.Where(x => (!string.IsNullOrEmpty(x.AuthorName) ?
                    x.AuthorName.Contains(searchString) : true)).ToList(); break;
                default: matchingBooks = allBooks; break;
            }

            return matchingBooks;
        }

        public async Task<List<BookResponse>> GetSortedBooks(List<BookResponse> allBooks, string sortBy, SortOrderOptions sortOrder)
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

        public async Task<BookResponse> UpdateBook(BookUpdateRequest bookUpdateRequest)
        {
            if (bookUpdateRequest == null) throw new ArgumentNullException(nameof(Book));
            ValidationHelper.ValidateModels(bookUpdateRequest);

            Book? book = await _BooksRepository.GetBookByBookId(bookUpdateRequest.BookId);
            if (book == null) throw new ArgumentException("Given book doesn't exist");

            // update matching returned book with bookUpdateRequest details | Entitystate.modified
            book.BookName = bookUpdateRequest.BookName;
            book.PublishedDate = bookUpdateRequest.PublishedDate;
            book.BookRating = bookUpdateRequest.BookRating;
            book.Publisher = bookUpdateRequest.Publisher;
            book.AuthorId = bookUpdateRequest.AuthorId;
            book.Genre = bookUpdateRequest.Genre.ToString();
            book.Genres = GenresListToString(bookUpdateRequest.GenresList);
            book.IsOngoing = bookUpdateRequest.IsOngoing;

            await _BooksRepository.UpdateBook(book); // save changes

            return book.ToBookResponse();
        }

        public string GenresListToString(List<string> GenresList) // convert genres list to string format
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < GenresList.Count; i++)
            {
                sb.Append(GenresList[i]);
                if (i < GenresList.Count - 1) sb.Append(", ");
            }

            return sb.ToString();
        }

        public async Task<bool> DeleteBook(Guid? bookId)
        {
            if (bookId == null) throw new ArgumentNullException(nameof(bookId));

            Book? book = await _BooksRepository.GetBookByBookId(bookId.Value); // check if book is valid
            if (book == null) return false;

            await _BooksRepository.DeleteBookByBookId(book.BookId);

            return true;
        }

        public async Task<MemoryStream> GetBooksCSV() // generates a csv file for the table data
        {
            MemoryStream memoryStream = new MemoryStream();

            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
            {
                CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

                csvWriter.WriteHeader<BookResponse>(); //PersonID,PersonName,...
                csvWriter.NextRecord();

                List<BookResponse> books = await GetAllBooks();

                books.AddRange(books);
                await csvWriter.WriteRecordsAsync(books);

                // Make sure to flush the writer
                await streamWriter.FlushAsync();
            }

            // Create a new MemoryStream and copy the data from the original stream
            MemoryStream newMemoryStream = new MemoryStream(memoryStream.ToArray());
            newMemoryStream.Position = 0;
            return newMemoryStream;
        }

        public async Task<MemoryStream> GetBooksExcel()
        {
            MemoryStream memoryStream = new();
            using (ExcelPackage excelPackage = new(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Book Id";
                workSheet.Cells["B1"].Value = "Book Name";
                workSheet.Cells["C1"].Value = "Book Rating";
                workSheet.Cells["D1"].Value = "Publisher";
                workSheet.Cells["E1"].Value = "Published Date";
                workSheet.Cells["F1"].Value = "Genre";
                workSheet.Cells["G1"].Value = "Author Id";
                workSheet.Cells["H1"].Value = "Author Name";
                workSheet.Cells["I1"].Value = "Status";

                using (ExcelRange headerCells = workSheet.Cells["A1:I1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<BookResponse> books = await GetAllBooks();
                foreach (BookResponse book in books)
                {
                    workSheet.Cells[row, 1].Value = book.BookId;
                    workSheet.Cells[row, 2].Value = book.BookName;
                    workSheet.Cells[row, 3].Value = book.BookRating;
                    workSheet.Cells[row, 4].Value = book.Publisher;
                    workSheet.Cells[row, 5].Value = book.PublishedDate.HasValue ? book.PublishedDate.Value.ToString("yyy-MM-dd") : "";
                    workSheet.Cells[row, 6].Value = GenresListToString(book.Genress);
                    workSheet.Cells[row, 7].Value = book.AuthorId;
                    workSheet.Cells[row, 8].Value = book.AuthorName;
                    workSheet.Cells[row, 9].Value = book.IsOngoing.HasValue ? "Ongoing" : "Completed";

                    row += 1;
                }

                workSheet.Cells[$"A1:I{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
