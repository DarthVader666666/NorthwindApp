using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Renamed_Employee_table_and_FK_to_Seller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTerritories_Employees",
                table: "EmployeeTerritories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTerritories_Territories",
                table: "EmployeeTerritories");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeTerritories",
                table: "EmployeeTerritories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "EmployeeTerritories",
                newName: "SellerTerritories");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Sellers");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "Orders",
                newName: "SellerID");

            migrationBuilder.RenameIndex(
                name: "EmployeesOrders",
                table: "Orders",
                newName: "SellersOrders");

            migrationBuilder.RenameIndex(
                name: "EmployeeID",
                table: "Orders",
                newName: "SellerID");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "SellerTerritories",
                newName: "SellerID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_EmployeeTerritories_TerritoryID",
            //    table: "SellerTerritories",
            //    newName: "IX_SellerTerritories_TerritoryID");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "Sellers",
                newName: "SellerID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Employees_ReportsTo",
            //    table: "Sellers",
            //    newName: "IX_Sellers_ReportsTo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SellerTerritories",
                table: "SellerTerritories",
                columns: new[] { "SellerID", "TerritoryID" })
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sellers",
                table: "Sellers",
                column: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sellers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                principalTable: "Sellers",
                principalColumn: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Sellers",
                table: "Orders",
                column: "SellerID",
                principalTable: "Sellers",
                principalColumn: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sellers_Sellers",
                table: "Sellers",
                column: "ReportsTo",
                principalTable: "Sellers",
                principalColumn: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerTerritories_Sellers",
                table: "SellerTerritories",
                column: "SellerID",
                principalTable: "Sellers",
                principalColumn: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerTerritories_Territories",
                table: "SellerTerritories",
                column: "TerritoryID",
                principalTable: "Territories",
                principalColumn: "TerritoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sellers_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Sellers",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Sellers_Sellers",
                table: "Sellers");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerTerritories_Sellers",
                table: "SellerTerritories");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerTerritories_Territories",
                table: "SellerTerritories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SellerTerritories",
                table: "SellerTerritories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sellers",
                table: "Sellers");

            migrationBuilder.RenameTable(
                name: "SellerTerritories",
                newName: "EmployeeTerritories");

            migrationBuilder.RenameTable(
                name: "Sellers",
                newName: "Employees");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Orders",
                newName: "EmployeeID");

            migrationBuilder.RenameIndex(
                name: "SellersOrders",
                table: "Orders",
                newName: "EmployeesOrders");

            migrationBuilder.RenameIndex(
                name: "SellerID",
                table: "Orders",
                newName: "EmployeeID");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "EmployeeTerritories",
                newName: "EmployeeID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_SellerTerritories_TerritoryID",
            //    table: "EmployeeTerritories",
            //    newName: "IX_EmployeeTerritories_TerritoryID");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Employees",
                newName: "EmployeeID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Sellers_ReportsTo",
            //    table: "Employees",
            //    newName: "IX_Employees_ReportsTo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeTerritories",
                table: "EmployeeTerritories",
                columns: new[] { "EmployeeID", "TerritoryID" })
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees",
                table: "Employees",
                column: "ReportsTo",
                principalTable: "Employees",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTerritories_Employees",
                table: "EmployeeTerritories",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTerritories_Territories",
                table: "EmployeeTerritories",
                column: "TerritoryID",
                principalTable: "Territories",
                principalColumn: "TerritoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees",
                table: "Orders",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID");
        }
    }
}
