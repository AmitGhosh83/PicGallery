using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeManagement.DataAccess.Migrations
{
    public partial class SeedEmployeeTablefromModelBuilder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "EmailId", "Name" },
                values: new object[] { 2, 1, "linda@gmail.com", "Linda" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "EmailId", "Name" },
                values: new object[] { 3, 3, "amit@gmail.com", "Amit" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
