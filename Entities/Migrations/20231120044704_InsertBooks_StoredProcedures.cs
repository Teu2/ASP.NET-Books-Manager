using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertBooks_StoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // sql statement
            string sp_AddBooks = @"
                CREATE PROCEDURE [dbo].[AddBooks]
                (@BookId uniqueidentifier, @BookRating int, @BookName nvarchar(200), @Publisher nvarchar(100), @PublishedDate datetime2(7), @Genre nvarchar(max), @Genres nvarchar(max), @AuthorId uniqueidentifier, @IsOngoing bit)
                AS BEGIN
                    INSERT INTO [dbo].[Books](BookId, BookRating, BookName, Publisher, PublishedDate, Genre, Genres, AuthorId, IsOngoing) VALUES (@BookId, @BookRating, @BookName, @Publisher, @PublishedDate, @Genre, @Genres, @AuthorId, @IsOngoing)
                END
            ";

            migrationBuilder.Sql(sp_AddBooks);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // down undoes migration
            string sp_AddBooks = @"
                DROP PROCEDURE [dbo].[AddBooks]
            ";

            migrationBuilder.Sql(sp_AddBooks);
        }
    }
}
