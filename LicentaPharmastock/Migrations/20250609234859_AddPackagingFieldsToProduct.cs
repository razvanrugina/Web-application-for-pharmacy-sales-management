using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicentaPharmastock.Migrations
{
    /// <inheritdoc />
    public partial class AddPackagingFieldsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerPassword");

            migrationBuilder.DropColumn(
                name: "Prospectus",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "price",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Product",
                newName: "UnitsPerPackage");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "LooseUnitCount",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackageCount",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PackagePrice",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LooseUnitCount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PackageCount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PackagePrice",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "UnitsPerPackage",
                table: "Product",
                newName: "quantity");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Product",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Prospectus",
                table: "Product",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "price",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "ManagerPassword",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerPassword", x => x.Id);
                });
        }
    }
}
