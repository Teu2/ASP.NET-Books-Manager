using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Key]
        public Guid BookId { get; set; } // ISBN International Standard Book Number
        
        public int? BookRating { get; set; }
        
        [StringLength(200)]
        public string? BookName { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; } // email or name?
        
        public DateTime? PublishedDate { get; set; }
        
        public string? Genre { get; set; }
        
        public string? Genres { get; set; }
        
        public Guid? AuthorId { get; set; }
        
        public bool? IsOngoing { get; set; }
    }
}
