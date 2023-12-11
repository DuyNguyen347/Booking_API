using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPosterTableAndAddCinameImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hotline",
                table: "Cinemas",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "latitude",
                table: "Cinemas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "longitude",
                table: "Cinemas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CinemaImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cinemaid = table.Column<long>(name: "cinema_id", type: "bigint", nullable: false),
                    namefile = table.Column<string>(name: "name_file", type: "varchar(MAX)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pathimage = table.Column<string>(name: "path-image", type: "nvarchar(MAX)", nullable: false),
                    linkurl = table.Column<string>(name: "link-url", type: "varchar(MAX)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CinemaImages");

            migrationBuilder.DropTable(
                name: "Posters");

            migrationBuilder.DropColumn(
                name: "hotline",
                table: "Cinemas");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "Cinemas");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "Cinemas");
        }
    }
}
