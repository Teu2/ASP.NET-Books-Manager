using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;
using Entities;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO for adding a new book
    /// </summary>
    public class BookAddRequest
    {
        // Properties 
        [Required(ErrorMessage = "Book name must not be empty")]
        public string? BookName { get; set; }

        [Required(ErrorMessage = "Book must have a rating")]
        public int? BookRating { get; set; }

        [Required(ErrorMessage = "Publisher name must not be empty")]
        public string? Publisher { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; }

        [Required(ErrorMessage = "Genre must not be empty")]
        public GenreOptions Genre { get; set; }

        [Required(ErrorMessage = "Genres must not be empty")]
        public string? Genres { get; set; }

        public Guid AuthorId { get; set; }

        //[Required(ErrorMessage = "Ongoing must be true or false")]
        public bool? IsOngoing { get; set; }

        public Book ToBook() // convert BookAddRequest to a Book object type
        {
            return new Book() // return new book
            {
                BookName = BookName,
                BookRating = BookRating,

                Publisher = Publisher,
                PublishedDate = PublishedDate,

                Genre = Genre.ToString(),
                Genres = Genres,

                AuthorId = AuthorId,

                IsOngoing = IsOngoing
            };
        }
    }
}
