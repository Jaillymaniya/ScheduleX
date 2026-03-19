using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxSemToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblUser_Email",
                table: "TblUser");

            migrationBuilder.DropIndex(
                name: "IX_TblUser_Phone",
                table: "TblUser");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "TblUser",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TblUser",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MaxSem",
                table: "TblCourse",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_TblUser_Email",
                table: "TblUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblUser_Phone",
                table: "TblUser",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblUser_Email",
                table: "TblUser");

            migrationBuilder.DropIndex(
                name: "IX_TblUser_Phone",
                table: "TblUser");

            migrationBuilder.DropColumn(
                name: "MaxSem",
                table: "TblCourse");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "TblUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TblUser",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateIndex(
                name: "IX_TblUser_Email",
                table: "TblUser",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblUser_Phone",
                table: "TblUser",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");
        }
    }
}
