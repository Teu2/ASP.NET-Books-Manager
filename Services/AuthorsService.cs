using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services
{
    public class AuthorsService : IAuthorsService
    {
        // private field
        private readonly IAuthorsRepository _authorsRepository;

        public AuthorsService(IAuthorsRepository authorsRepository) // default init value is true
        {
            _authorsRepository = authorsRepository;
        }

        public async Task<AuthorResponse> AddAuthor(AuthorAddRequest? authorAddRequest)
        {
            // null checks
            if (authorAddRequest == null) throw new ArgumentNullException(nameof(authorAddRequest));
            if (authorAddRequest.AuthorName == null) throw new ArgumentNullException(nameof(authorAddRequest.AuthorName));
            if (await _authorsRepository.GetAuthorByAuthorName(authorAddRequest.AuthorName) != null) throw new ArgumentException("Author is already registered");

            // convert obj from AuthorAddRequest to Author type (DTO to domain model)
            Author author = authorAddRequest.ToAuthor(); 

            // assign new author id to author
            author.AuthorId = Guid.NewGuid(); 
            _authorsRepository.AddAuthor(author);

            // convert Author type to AuthorResponse type using injected method
            return author.ToAuthorResponse(); 
        }

        public async Task<List<AuthorResponse>> GetAllAuthors()
        {
            return (await _authorsRepository.GetAllAuthors())
                .Select(author => author.ToAuthorResponse())
                .ToList();
        }

        public async Task<AuthorResponse?> GetAuthorById(Guid? authorId)
        {
            if (authorId == null) return null;
            Author? authorRes = await _authorsRepository.GetAuthorByAuthorId((Guid)authorId);

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
                        if (await _authorsRepository.GetAuthorByAuthorName(authorName) == null)
                        {
                            Author author = new Author() { AuthorName = authorName };
                            await _authorsRepository.AddAuthor(author);

                            authorsInserted++;
                        }
                    }
                }
            }

            return authorsInserted;
        }
    }
}