using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Author
    {
        // domain model for author details
        [Key]
        public Guid AuthorId { get; set; } // AuthorId is primary key!

        public string? AuthorName { get; set; }
    }
}