using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFacultyFromTimtTableEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblTimeTableEntry_TblFaculty_FacultyId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropIndex(
                name: "IX_TblTimeTableEntry_FacultyId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "TblTimeTableEntry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "TblTimeTableEntry",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_FacultyId",
                table: "TblTimeTableEntry",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblTimeTableEntry_TblFaculty_FacultyId",
                table: "TblTimeTableEntry",
                column: "FacultyId",
                principalTable: "TblFaculty",
                principalColumn: "FacultyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
