using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.AuthorId);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookRating = table.Column<int>(type: "int", nullable: true),
                    BookName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Publisher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Genres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsOngoing = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "AuthorName" },
                values: new object[,]
                {
                    { new Guid("0e24b3f8-2167-43c7-8648-7442ba71e15a"), "Hirai Momo" },
                    { new Guid("48b6490a-4ddb-4606-99e4-f97641fbdcc7"), "Shin Ryuijin" },
                    { new Guid("6cb6a381-c136-4ac8-8476-0fcb20f06f2b"), "Lee Chae-Ryeong" },
                    { new Guid("7275844c-6d85-41ae-9292-9f4c66197492"), "Hwang Yeji" },
                    { new Guid("976cd651-ea40-4138-a68a-9efe92eb6337"), "Bae Joohyun" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "AuthorId", "BookName", "BookRating", "Genre", "Genres", "IsOngoing", "PublishedDate", "Publisher" },
                values: new object[,]
                {
                    { new Guid("1a895104-1e8f-4c05-9cf0-c09a3adcd3a9"), new Guid("6cb6a381-c136-4ac8-8476-0fcb20f06f2b"), "Book A", 4, null, "Action, Comedy", false, new DateTime(2023, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Feedbug" },
                    { new Guid("d798fc05-fe54-4a2e-ae82-9955a2dc0a56"), new Guid("48b6490a-4ddb-4606-99e4-f97641fbdcc7"), "Book B", 5, null, "Drama, Comedy, Adventure, Horror", true, new DateTime(2023, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "OTK" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
