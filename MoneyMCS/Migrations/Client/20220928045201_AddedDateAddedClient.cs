using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMCS.Migrations.Client
{
    public partial class AddedDateAddedClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Clients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 9, 28, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Clients");
        }
    }
}
