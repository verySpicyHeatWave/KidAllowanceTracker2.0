using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllowanceApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class NestedPersistenceFix3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_AccountID",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "PointObjects");

            migrationBuilder.DropTable(
                name: "Allowances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AccountID",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "AccountDetails");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Transactions",
                newName: "TransactionID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "AccountDetails",
                newName: "AccountID");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_Name",
                table: "AccountDetails",
                newName: "IX_AccountDetails_Name");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionID",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<double>(
                name: "BaseAllowance",
                table: "AccountDetails",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                columns: new[] { "AccountID", "TransactionID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountDetails",
                table: "AccountDetails",
                column: "AccountID");

            migrationBuilder.CreateTable(
                name: "AllowancePoints",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowancePoints", x => new { x.AccountID, x.Category });
                    table.ForeignKey(
                        name: "FK_AllowancePoints_AccountDetails_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AccountDetails",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AccountDetails_AccountID",
                table: "Transactions",
                column: "AccountID",
                principalTable: "AccountDetails",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AccountDetails_AccountID",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "AllowancePoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
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

            migrationBuilder.RenameColumn(
                name: "TransactionID",
                table: "Transactions",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "AccountID",
                table: "Accounts",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountDetails_Name",
                table: "Accounts",
                newName: "IX_Accounts_Name");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Allowances",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    BasePay = table.Column<double>(type: "REAL", nullable: false),
                    LastPayout = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allowances", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_Allowances_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointObjects",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointObjects", x => new { x.AccountID, x.Category });
                    table.ForeignKey(
                        name: "FK_PointObjects_Allowances_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Allowances",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountID",
                table: "Transactions",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_AccountID",
                table: "Transactions",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
