using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllowanceApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class NestedPersistenceFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PointObjects",
                table: "PointObjects");

            migrationBuilder.DropIndex(
                name: "IX_PointObjects_AccountID",
                table: "PointObjects");

            migrationBuilder.DropIndex(
                name: "IX_PointObjects_Category_Price",
                table: "PointObjects");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PointObjects",
                newName: "Points");

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "PointObjects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointObjects",
                table: "PointObjects",
                columns: new[] { "AccountID", "Category" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PointObjects",
                table: "PointObjects");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "PointObjects",
                newName: "ID");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "PointObjects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointObjects",
                table: "PointObjects",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_PointObjects_AccountID",
                table: "PointObjects",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_PointObjects_Category_Price",
                table: "PointObjects",
                columns: new[] { "Category", "Price" },
                unique: true);
        }
    }
}
