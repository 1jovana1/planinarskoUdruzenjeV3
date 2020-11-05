using Microsoft.EntityFrameworkCore.Migrations;

namespace planinarskoUdruzenjeV3.Migrations
{
    public partial class participationuserkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "participation",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_participation_user_id",
                table: "participation",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_participation_AspNetUsers_user_id",
                table: "participation",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_participation_AspNetUsers_user_id",
                table: "participation");

            migrationBuilder.DropIndex(
                name: "IX_participation_user_id",
                table: "participation");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "participation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
