using Entities;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IAuthorsService // interface business logic for manipulating country entity
    {
        AuthorResponse AddAuthor(AuthorAddRequest? authorAddRequest);

        /// <summary>
        /// Returns all registered Authors
        /// </summary>s
        /// <returns></returns>
        List<AuthorResponse> GetAllAuthors();

        /// <summary>
        /// returns Author obj depending on the passed author id
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        AuthorResponse? GetAuthorById(Guid? authorId);
    }
}