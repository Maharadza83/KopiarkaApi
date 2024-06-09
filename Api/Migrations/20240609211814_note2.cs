using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class note2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Notes",
                newName: "CreationDate");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Notes",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Notes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Notes",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notes",
                newName: "Category");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Notes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
