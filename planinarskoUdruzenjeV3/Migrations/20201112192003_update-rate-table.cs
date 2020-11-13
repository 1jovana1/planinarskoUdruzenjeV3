using Microsoft.EntityFrameworkCore.Migrations;

namespace planinarskoUdruzenjeV3.Migrations
{
    public partial class updateratetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "rate",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rate_user_id",
                table: "rate",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rate_AspNetUsers_user_id",
                table: "rate",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.RenameColumn(
                    name : "rate",
                    table : "rate",
                    newName : "stars"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rate_AspNetUsers_user_id",
                table: "rate");

            migrationBuilder.DropIndex(
                name: "IX_rate_user_id",
                table: "rate");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "rate",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "stars",
                table: "rate",
                newName: "rate"
            );
        }
    }
}
