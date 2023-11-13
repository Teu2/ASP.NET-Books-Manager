using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class used as a return type for most methods in BooksService
    /// </summary>
    public class BookResponse // used to display information to the user
    {
        // declaring properties of book to return to the controller or unit test case
        public Guid BookId { get; set; } // ISBN International Standard Book Number
        public string? BookName { get; set; }
        public int? BookRating { get; set; }
        public string? Publisher { get; set; } // email or name?
        public DateTime? PublishedDate { get; set; }
        public string? Genre { get; set; }
        public List<string>? Genress { get; set; }
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public bool? IsOngoing { get; set; }
        public double? BookAge { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(BookResponse)) return false;

            BookResponse book = (BookResponse)obj;

            return this.BookId == book.BookId && this.BookName == book.BookName && this.Publisher == book.Publisher && this.AuthorName == book.AuthorName;
        }

        public override string ToString()
        {
            return $"{BookId} - {BookName} - {Publisher} - {AuthorName}";
        }

        public BookUpdateRequest ToBookUpdateRequest()
        {
            return new BookUpdateRequest()
            {
                BookId = BookId,
                BookName = BookName,
                BookRating = BookRating,
                Publisher = Publisher,
                PublishedDate = PublishedDate,
                Genre = !string.IsNullOrEmpty(Genre)
                        ? (GenreOptions)Enum.Parse(typeof(GenreOptions), Genre, true)
                        : GenreOptions.Unknown,
                Genres = Genress.ToString(),
                AuthorId = AuthorId,
                IsOngoing = IsOngoing,
            };
        }
    }

    /// <summary>
    /// extension method to convert Book class object into BookResponse object type
    /// </summary>
    public static class BookExtensions
    {
        public static BookResponse ToBookResponse(this Book book) 
        {
            return new BookResponse()
            {
                BookId = book.BookId,
                BookName = book.BookName,
                BookRating = book.BookRating,
                Publisher = book.Publisher,
                PublishedDate = book.PublishedDate,
                Genre = book.Genre,
                Genress = ConvertToListString(book.Genres),
                AuthorId = (Guid)book.AuthorId,
                IsOngoing = book.IsOngoing,
                BookAge = GetBookAge(book)
            };
        }

        public static List<string> ConvertToListString(string Genres)
        {
            // split string into an array using comma delimiter
            string[] genreArray = Genres.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> genreList = genreArray.ToList(); // convert  array to a List<string>
            return genreList;
        }

        private static double GetBookAge(Book book)
        {
            return (book.PublishedDate != null) ? Math.Round((DateTime.Now - book.PublishedDate.Value).TotalDays / 365.25) : 0;
        }
    }
}