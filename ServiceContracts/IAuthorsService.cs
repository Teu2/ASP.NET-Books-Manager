using Entities;
using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    public interface IAuthorsService // interface business logic for manipulating country entity
    {
        Task<AuthorResponse> AddAuthor(AuthorAddRequest? authorAddRequest);

        /// <summary>
        /// Returns all registered Authors
        /// </summary>s
        /// <returns></returns>
        Task<List<AuthorResponse>> GetAllAuthors();

        /// <summary>
        /// returns Author obj depending on the passed author id
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        Task<AuthorResponse?> GetAuthorById(Guid? authorId);

        Task<int> UploadAuthorsFromExcel(IFormFile formFile);
    }
}