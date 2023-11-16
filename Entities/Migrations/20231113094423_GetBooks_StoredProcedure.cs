using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetBooks_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // sql statement
            string sp_GetAllBooks = @"
                CREATE PROCEDURE [dbo].[GetAllBooks]
                AS BEGIN
                    SELECT BookId, BookName, BookRating, Publisher, PublishedDate, Genre, Genres, AuthorId, IsOngoing FROM [dbo].[Books]
                END
            ";

            migrationBuilder.Sql(sp_GetAllBooks);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // down undoes migration
            string sp_GetAllBooks = @"
                DROP PROCEDURE [dbo].[GetAllBooks]
            ";

            migrationBuilder.Sql(sp_GetAllBooks);
        }
    }
}
