using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addmodelforpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "booking_content",
                table: "booking",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "booking_currency",
                table: "booking",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "booking_date",
                table: "booking",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "booking_destination_id",
                table: "booking",
                type: "varchar(60)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "booking_language",
                table: "booking",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "booking_message",
                table: "booking",
                type: "varchar(60)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "booking_ref_id",
                table: "booking",
                type: "varchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "booking_status",
                table: "booking",
                type: "nvarchar(30)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expire_date",
                table: "booking",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "merchant_id",
                table: "booking",
                type: "varchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "paid_amount",
                table: "booking",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "required_amount",
                table: "booking",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "booking_content",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "booking_currency",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "booking_date",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "booking_destination_id",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "booking_language",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "booking_message",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "booking_ref_id",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "booking_status",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "expire_date",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "merchant_id",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "paid_amount",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "required_amount",
                table: "booking");
        }
    }
}
