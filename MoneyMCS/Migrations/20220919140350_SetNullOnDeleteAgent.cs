using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMCS.Migrations
{
    public partial class SetNullOnDeleteAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ReferrerId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ReferrerId",
                table: "AspNetUsers",
                column: "ReferrerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ReferrerId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ReferrerId",
                table: "AspNetUsers",
                column: "ReferrerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
