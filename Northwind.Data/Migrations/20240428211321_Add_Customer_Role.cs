using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Customer_Role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $"INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) " +
                $"VALUES('ab174f91-bbdd-4c07-b476-d40c997fd4c9','customer', 'CUSTOMER', null)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}