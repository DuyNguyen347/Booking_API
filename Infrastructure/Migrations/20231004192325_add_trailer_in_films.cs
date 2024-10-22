using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addtrailerinfilms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "film_id",
                table: "FilmImages",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "trailer",
                table: "Films",
                type: "nvarchar(MAX)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trailer",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FilmImages",
                newName: "film_id");
        }
    }
}
