using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class AuthorsRepository : IAuthorsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthorsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Author> AddAuthor(Author author)
        {
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync(); // verifies the objects added into the db set

            return author;
        }

        public async Task<List<Author>> GetAllAuthors()
        {
           return await _dbContext.Authors.ToListAsync();
        }

        public async Task<Author?> GetAuthorByAuthorId(Guid authorId)
        {
            return await _dbContext.Authors.FirstOrDefaultAsync(temp => temp.AuthorId == authorId);
        }

        public async Task<Author?> GetAuthorByAuthorName(string authorName)
        {
            return await _dbContext.Authors.FirstOrDefaultAsync(temp => temp.AuthorName == authorName);
        }
    }
}