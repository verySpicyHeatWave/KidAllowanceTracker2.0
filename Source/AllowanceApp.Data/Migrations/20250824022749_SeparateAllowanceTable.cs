using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllowanceApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeparateAllowanceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointType_Accounts_AccountID",
                table: "PointType");

            migrationBuilder.DropColumn(
                name: "Allowance_BasePay",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Allowance_ID",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Allowance_LastPayout",
                table: "Accounts");

            migrationBuilder.CreateTable(
                name: "Allowances",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    ID = table.Column<int>(type: "INTEGER", nullable: false),
                    BasePay = table.Column<double>(type: "REAL", nullable: false, defaultValue: 5.0),
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

            migrationBuilder.AddForeignKey(
                name: "FK_PointType_Allowances_AccountID",
                table: "PointType",
                column: "AccountID",
                principalTable: "Allowances",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointType_Allowances_AccountID",
                table: "PointType");

            migrationBuilder.DropTable(
                name: "Allowances");

            migrationBuilder.AddColumn<double>(
                name: "Allowance_BasePay",
                table: "Accounts",
                type: "REAL",
                nullable: false,
                defaultValue: 5.0);

            migrationBuilder.AddColumn<int>(
                name: "Allowance_ID",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Allowance_LastPayout",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddForeignKey(
                name: "FK_PointType_Accounts_AccountID",
                table: "PointType",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
