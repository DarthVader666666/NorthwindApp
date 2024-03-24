using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Northwind.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class Database_Seed : Migration
    {
        public Database_Seed()
        {
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var path = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") switch
            {
                "Local" => "..\\Northwind.Data\\SQL_Scripts\\Downloaded_Script.sql",
                _ => "../src/Northwind.Data/SQL_Scripts/Downloaded_Script.sql",
            };

            migrationBuilder.Sql(File.ReadAllText(path));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
