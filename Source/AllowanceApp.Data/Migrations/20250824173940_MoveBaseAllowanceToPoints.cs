using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllowanceApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveBaseAllowanceToPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllowancePoints_AccountDetails_AccountID",
                table: "AllowancePoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AccountDetails_AccountID",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountDetails",
                table: "AccountDetails");

            migrationBuilder.DropColumn(
                name: "BaseAllowance",
                table: "AccountDetails");

            migrationBuilder.RenameTable(
                name: "AccountDetails",
                newName: "Accounts");

            migrationBuilder.RenameIndex(
                name: "IX_AccountDetails_Name",
                table: "Accounts",
                newName: "IX_Accounts_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowancePoints_Accounts_AccountID",
                table: "AllowancePoints",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_AccountID",
                table: "Transactions",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllowancePoints_Accounts_AccountID",
                table: "AllowancePoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_AccountID",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "AccountDetails");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_Name",
                table: "AccountDetails",
                newName: "IX_AccountDetails_Name");

            migrationBuilder.AddColumn<double>(
                name: "BaseAllowance",
                table: "AccountDetails",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountDetails",
                table: "AccountDetails",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowancePoints_AccountDetails_AccountID",
                table: "AllowancePoints",
                column: "AccountID",
                principalTable: "AccountDetails",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AccountDetails_AccountID",
                table: "Transactions",
                column: "AccountID",
                principalTable: "AccountDetails",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
