using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicentaPharmastock.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Brand",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_name",
                table: "Brand",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Brand_name",
                table: "Brand");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
