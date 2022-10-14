using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMCS.Migrations
{
    public partial class AddedTransactionTypeEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTransaction_AspNetUsers_ApplicationUserId",
                table: "AppTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppTransaction",
                table: "AppTransaction");

            migrationBuilder.RenameTable(
                name: "AppTransaction",
                newName: "AppTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_AppTransaction_ApplicationUserId",
                table: "AppTransactions",
                newName: "IX_AppTransactions_ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "AppTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppTransactions",
                table: "AppTransactions",
                column: "AppTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTransactions_AspNetUsers_ApplicationUserId",
                table: "AppTransactions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTransactions_AspNetUsers_ApplicationUserId",
                table: "AppTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppTransactions",
                table: "AppTransactions");

            migrationBuilder.RenameTable(
                name: "AppTransactions",
                newName: "AppTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_AppTransactions_ApplicationUserId",
                table: "AppTransaction",
                newName: "IX_AppTransaction_ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "AppTransaction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppTransaction",
                table: "AppTransaction",
                column: "AppTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTransaction_AspNetUsers_ApplicationUserId",
                table: "AppTransaction",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
