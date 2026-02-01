using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFanShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePaymentDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }
    }
}
