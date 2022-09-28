using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMCS.Migrations.Client
{
    public partial class ChangeFKReferrerNameClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AgentUser_AgentUserId",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "AgentUserId",
                table: "Clients",
                newName: "ReferrerId");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_AgentUserId",
                table: "Clients",
                newName: "IX_Clients_ReferrerId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                table: "Clients",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 9, 28, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AgentUser_ReferrerId",
                table: "Clients",
                column: "ReferrerId",
                principalTable: "AgentUser",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AgentUser_ReferrerId",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "ReferrerId",
                table: "Clients",
                newName: "AgentUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_ReferrerId",
                table: "Clients",
                newName: "IX_Clients_AgentUserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                table: "Clients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 9, 28, 0, 0, 0, 0, DateTimeKind.Utc),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AgentUser_AgentUserId",
                table: "Clients",
                column: "AgentUserId",
                principalTable: "AgentUser",
                principalColumn: "Id");
        }
    }
}
