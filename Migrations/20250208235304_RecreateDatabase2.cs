using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace task_amwag.Migrations
{
    /// <inheritdoc />
    public partial class RecreateDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "TotalLeaveDays",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Name",
                table: "Employees",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Name",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TotalLeaveDays",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
