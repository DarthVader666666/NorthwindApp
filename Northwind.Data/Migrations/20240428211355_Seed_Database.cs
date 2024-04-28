using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(File.ReadAllText("Downloaded_Script.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
