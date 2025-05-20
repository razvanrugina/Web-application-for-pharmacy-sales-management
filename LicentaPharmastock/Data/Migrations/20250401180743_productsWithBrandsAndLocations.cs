using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicentaPharmastock.Data.Migrations
{
    /// <inheritdoc />
    public partial class productsWithBrandsAndLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Brand",
                newName: "name");

            migrationBuilder.AddColumn<float>(
                name: "price",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Brand",
                newName: "Name");
        }
    }
}
