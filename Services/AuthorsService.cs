using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
    public class AuthorsService : IAuthorsService
    {
        // private field
        private readonly List<Author> _authors;

        public AuthorsService(bool init = true) // default init value is true
        {
            _authors = new List<Author>();

            if (init)
            {
                _authors.AddRange(new List<Author>() 
                {
                    new Author() { AuthorId = Guid.Parse("6CB6A381-C136-4AC8-8476-0FCB20F06F2B"), AuthorName = "Kang Seulgi" },
                    new Author() { AuthorId = Guid.Parse("976CD651-EA40-4138-A68A-9EFE92EB6337"), AuthorName = "Bae Joohyun" },
                    new Author() { AuthorId = Guid.Parse("7275844C-6D85-41AE-9292-9F4C66197492"), AuthorName = "Emily Beth" },
                    new Author() { AuthorId = Guid.Parse("48B6490A-4DDB-4606-99E4-F97641FBDCC7"), AuthorName = "Harsha II" },
                    new Author() { AuthorId = Guid.Parse("0E24B3F8-2167-43C7-8648-7442BA71E15A"), AuthorName = "Harsha I" },
                });
            }
        }

        public AuthorResponse AddAuthor(AuthorAddRequest? authorAddRequest)
        {
            // null checks
            if (authorAddRequest == null) throw new ArgumentNullException(nameof(authorAddRequest));
            if (authorAddRequest.AuthorName == null) throw new ArgumentNullException(nameof(authorAddRequest.AuthorName));
            if (_authors.Where(c => c.AuthorName == authorAddRequest.AuthorName).Count() > 0) throw new ArgumentException("Author is already registered");
            
            // Convert obj from AuthorAddRequest to Author type (DTO to domain model)
            Author author = authorAddRequest.ToAuthor();
            
            author.AuthorId = Guid.NewGuid(); // assign new author id to author
            _authors.Add(author);

            return author.ToAuthorResponse(); // convert Author type to AuthorResponse type using injected method
        }

        public List<AuthorResponse> GetAllAuthors()
        {
            List<AuthorResponse> list = new();

            foreach(var author in _authors)
            {
                list.Add(author.ToAuthorResponse());
            }

            return list;

            // return _authors.Select(author => author.ToAuthorResponse()).ToList(); // works too
        }

        public AuthorResponse? GetAuthorById(Guid? authorId)
        {
            if (authorId == null) return null;

            Author? authorRes = _authors.FirstOrDefault(n => n.AuthorId == authorId);
            if (authorRes == null) return null;

            return authorRes.ToAuthorResponse();
        }
    }
}