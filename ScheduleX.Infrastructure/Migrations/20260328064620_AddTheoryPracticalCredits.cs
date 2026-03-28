using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTheoryPracticalCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Credits",
                table: "TblSubject",
                newName: "TheoryCredits");

            migrationBuilder.AddColumn<int>(
                name: "PracticalCredits",
                table: "TblSubject",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "TblFaculty",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PracticalCredits",
                table: "TblSubject");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "TblFaculty");

            migrationBuilder.RenameColumn(
                name: "TheoryCredits",
                table: "TblSubject",
                newName: "Credits");
        }
    }
}
