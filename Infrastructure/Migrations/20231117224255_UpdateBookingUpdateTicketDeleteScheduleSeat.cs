using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingUpdateTicketDeleteScheduleSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleSeats");

            migrationBuilder.DropColumn(
                name: "qr_code",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "schedule_id",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "seat_id",
                table: "ticket");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "ticket",
                newName: "booking_id");

            migrationBuilder.AddColumn<int>(
                name: "number",
                table: "ticket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "seatcode",
                table: "ticket",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "number",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "seatcode",
                table: "ticket");

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

            migrationBuilder.AddColumn<long>(
                name: "seat_id",
                table: "ticket",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ScheduleSeats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reservationtime = table.Column<DateTime>(name: "reservation_time", type: "datetime", nullable: true),
                    scheduleid = table.Column<long>(name: "schedule_id", type: "bigint", nullable: false),
                    seatid = table.Column<long>(name: "seat_id", type: "bigint", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleSeats", x => x.Id);
                });
        }
    }
}
