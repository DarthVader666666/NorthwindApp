using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Migrations.NorthwindIdentityDb
{
    /// <inheritdoc />
    public partial class Add_Admin_Role_and_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(File.ReadAllText("AdminScript.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
