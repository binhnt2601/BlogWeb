using System;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using App.Models;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                });
            //Insert Data
            //Seed Data: Bogus

            Randomizer.Seed = new Random(8675309);
            var seedArticle = new Faker<Article>();
            seedArticle.RuleFor(a => a.Title, f => f.Lorem.Sentence(10, 5));
            seedArticle.RuleFor(a => a.CreatedAt, f => f.Date.Between(new DateTime(2023, 1, 1), new DateTime(2023, 5, 25)));
            seedArticle.RuleFor(a => a.Content, f => f.Lorem.Paragraphs(1, 4));

            for(int i = 0; i<100; i++)
            {
                Article article = seedArticle.Generate();
                migrationBuilder.InsertData(
                table: "articles",
                columns: new[] {"Title", "CreatedAt", "Content"},
                values: new object[] {
                    article.Title, article.CreatedAt, article.Content
                }
            );
            }
            

            
            // migrationBuilder.InsertData(
            //     table: "articles",
            //     columns: new[] {"Title", "CreatedAt", "Content"},
            //     values: new object[] {
            //         "Post 2", DateTime.Now.Date, "Content 2"
            //     }
            // );
            // migrationBuilder.InsertData(
            //     table: "articles",
            //     columns: new[] {"Title", "CreatedAt", "Content"},
            //     values: new object[] {
            //         "Post 3", DateTime.Now.Date, "Content 3"
            //     }
            // );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
