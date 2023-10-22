using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServiceContracts.DTO
{
    public class AuthorResponse // DTO used as a return type for most of IAutherServices methods.
    {
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }

        /// <summary>
        /// compares current obj to another obj of AuthorResponse type and returns true if both values are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) // overrirde equals method for AuthorReponse objectss
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(AuthorResponse)) return false;

            AuthorResponse authorRes = (AuthorResponse)obj; // type cast object to AuthorResponse type
            return AuthorId == authorRes.AuthorId && AuthorName == authorRes.AuthorName;
        }
    }

    public static class AuthorExtensions
    {
        // add extension method to inject into Author class - converts Author object to AuthorResponse object type
        public static AuthorResponse ToAuthorResponse(this Author author)
        {
            return new AuthorResponse() { AuthorId = author.AuthorId, AuthorName = author.AuthorName };
        }
    }
}
