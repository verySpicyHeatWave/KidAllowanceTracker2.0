using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllowanceApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAllowanceID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ID",
                table: "Allowances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "Allowances",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
