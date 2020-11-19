using Microsoft.EntityFrameworkCore.Migrations;

namespace planinarskoUdruzenjeV3.Migrations
{
    public partial class news_alter_created_by : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "stars",
                table: "rate",
                newName: "stars");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "news",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "stars",
                table: "rate",
                newName: "rate");

            migrationBuilder.AlterColumn<int>(
                name: "created_by",
                table: "news",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
