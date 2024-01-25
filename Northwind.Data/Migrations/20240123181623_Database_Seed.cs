using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Northwind.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class Database_Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker")
            {
                migrationBuilder.Sql(File.ReadAllText("../src/Northwind.Data/SQL_Scripts/DB_Create_Script.sql"));
            }
            else 
            {
                migrationBuilder.Sql(File.ReadAllText("..\\Northwind.Data\\SQL_Scripts\\DB_Create_Script.sql"));
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
