using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Book domain model class used to store information we want into our db table
    /// </summary>
    public class Book
    {
        public Guid BookId { get; set; } // ISBN International Standard Book Number
        public int? BookRating { get; set; }
        public string? BookName { get; set; }
        public string? Publisher { get; set; } // email or name?
        public DateTime? PublishedDate { get; set; }
        public string? Genre { get; set; }
        public List<string>? Genres { get; set; }
        public Guid AuthorId { get; set; }
        public bool? IsOngoing { get; set; }
    }
}
