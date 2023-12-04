using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Services
{
    public class AuthorsService : IAuthorsService
    {
        // private field
        private readonly List<Author> _authors;
        private readonly ApplicationDbContext _dbContext;

        public AuthorsService(ApplicationDbContext booksDbContext) // default init value is true
        {
            _dbContext = booksDbContext;
        }

        public async Task<AuthorResponse> AddAuthor(AuthorAddRequest? authorAddRequest)
        {
            // null checks
            if (authorAddRequest == null) throw new ArgumentNullException(nameof(authorAddRequest));
            if (authorAddRequest.AuthorName == null) throw new ArgumentNullException(nameof(authorAddRequest.AuthorName));
            if (await _dbContext.Authors.CountAsync(c => c.AuthorName == authorAddRequest.AuthorName) > 0) throw new ArgumentException("Author is already registered");

            // convert obj from AuthorAddRequest to Author type (DTO to domain model)
            Author author = authorAddRequest.ToAuthor(); 

            // assign new author id to author
            author.AuthorId = Guid.NewGuid(); 
            _dbContext.Authors.Add(author);

            await _dbContext.SaveChangesAsync(); // verifies the objects added into the db set

            // convert Author type to AuthorResponse type using injected method
            return author.ToAuthorResponse(); 
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

        public async Task<int> UploadAuthorsFromExcel(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            int authorsInserted = 0; // int to return for message in Authors controller

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                // reads from worksheet called "Authors"
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Authors"]; 
                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    // gets each value under the first column
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue)) // check if the cell has a value
                    {
                        string? authorName = cellValue;

                        //if the author doesn't exist add it into the dbContext
                        if (_dbContext.Authors.Where(temp => temp.AuthorName == authorName).Count() == 0)
                        {
                            Author author = new Author() { AuthorName = authorName };
                            _dbContext.Authors.Add(author);
                            await _dbContext.SaveChangesAsync();

                            authorsInserted++;
                        }
                    }
                }
            }

            return authorsInserted;
        }
    }
}