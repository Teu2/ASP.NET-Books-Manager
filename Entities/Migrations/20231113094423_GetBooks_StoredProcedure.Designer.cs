﻿// <auto-generated />
using System;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Entities.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231113094423_GetBooks_StoredProcedure")]
    partial class GetBooks_StoredProcedure
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Entities.Author", b =>
                {
                    b.Property<Guid>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuthorName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors", (string)null);

                    b.HasData(
                        new
                        {
                            AuthorId = new Guid("6cb6a381-c136-4ac8-8476-0fcb20f06f2b"),
                            AuthorName = "Lee Chae-Ryeong"
                        },
                        new
                        {
                            AuthorId = new Guid("976cd651-ea40-4138-a68a-9efe92eb6337"),
                            AuthorName = "Bae Joohyun"
                        },
                        new
                        {
                            AuthorId = new Guid("7275844c-6d85-41ae-9292-9f4c66197492"),
                            AuthorName = "Hwang Yeji"
                        },
                        new
                        {
                            AuthorId = new Guid("48b6490a-4ddb-4606-99e4-f97641fbdcc7"),
                            AuthorName = "Shin Ryuijin"
                        },
                        new
                        {
                            AuthorId = new Guid("0e24b3f8-2167-43c7-8648-7442ba71e15a"),
                            AuthorName = "Hirai Momo"
                        });
                });

            modelBuilder.Entity("Entities.Book", b =>
                {
                    b.Property<Guid>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BookName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int?>("BookRating")
                        .HasColumnType("int");

                    b.Property<string>("Genre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Genres")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsOngoing")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Publisher")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("BookId");

                    b.ToTable("Books", (string)null);

                    b.HasData(
                        new
                        {
                            BookId = new Guid("1a895104-1e8f-4c05-9cf0-c09a3adcd3a9"),
                            AuthorId = new Guid("6cb6a381-c136-4ac8-8476-0fcb20f06f2b"),
                            BookName = "Book A",
                            BookRating = 4,
                            Genres = "Action, Comedy",
                            IsOngoing = false,
                            PublishedDate = new DateTime(2023, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Publisher = "Feedbug"
                        },
                        new
                        {
                            BookId = new Guid("d798fc05-fe54-4a2e-ae82-9955a2dc0a56"),
                            AuthorId = new Guid("48b6490a-4ddb-4606-99e4-f97641fbdcc7"),
                            BookName = "Book B",
                            BookRating = 5,
                            Genres = "Drama, Comedy, Adventure, Horror",
                            IsOngoing = true,
                            PublishedDate = new DateTime(2023, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Publisher = "OTK"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
