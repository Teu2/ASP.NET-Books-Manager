using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServiceContracts.DTO
{
    public class AuthorAddRequest // DTO Class to add new authors
    {
        public string? AuthorName { get; set; }

        public Author ToAuthor()
        {
            return new Author() { AuthorName = AuthorName };
        }
    }
}
