using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class AuthorsService : IAuthorsService
    {
        // private field
        private readonly List<Author> _authors;
        private readonly BooksDbContext _dbContext;

        public AuthorsService(BooksDbContext booksDbContext) // default init value is true
        {
            _dbContext = booksDbContext;
        }

        public async Task<AuthorResponse> AddAuthor(AuthorAddRequest? authorAddRequest)
        {
            // null checks
            if (authorAddRequest == null) throw new ArgumentNullException(nameof(authorAddRequest));
            if (authorAddRequest.AuthorName == null) throw new ArgumentNullException(nameof(authorAddRequest.AuthorName));
            if (await _dbContext.Authors.CountAsync(c => c.AuthorName == authorAddRequest.AuthorName) > 0) throw new ArgumentException("Author is already registered");
            
            // Convert obj from AuthorAddRequest to Author type (DTO to domain model)
            Author author = authorAddRequest.ToAuthor();
            
            author.AuthorId = Guid.NewGuid(); // assign new author id to author
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync(); // verifies the objects added into the db set

            return author.ToAuthorResponse(); // convert Author type to AuthorResponse type using injected method
        }

        public async Task<List<AuthorResponse>> GetAllAuthors()
        {
            return await _dbContext.Authors.Select(author => author.ToAuthorResponse()).ToListAsync();
        }

        public async Task<AuthorResponse?> GetAuthorById(Guid? authorId)
        {
            if (authorId == null) return null;
            Author? authorRes = await _dbContext.Authors.FirstOrDefaultAsync(c => c.AuthorId == authorId);

            return authorRes.ToAuthorResponse();
        }
    }
}