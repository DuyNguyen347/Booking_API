using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class rmSeatStatusaddScheduleSeatchangeTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Seats");

            migrationBuilder.RenameColumn(
                name: "booking_id",
                table: "ticket",
                newName: "user_id");

            migrationBuilder.AddColumn<byte[]>(
                name: "qr_code",
                table: "ticket",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "schedule_id",
                table: "ticket",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "qr_code",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "schedule_id",
                table: "ticket");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "ticket",
                newName: "booking_id");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
