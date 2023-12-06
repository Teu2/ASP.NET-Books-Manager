using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace RepositoryContracts
{
    public interface IAuthorsRepository
    {
        Task<Author> AddAuthor(Author author);

        Task<List<Author>> GetAllAuthors();

        Task<Author?> GetAuthorByAuthorId(Guid authorId);

        Task<Author?> GetAuthorByAuthorName(string authorName);
    }
}