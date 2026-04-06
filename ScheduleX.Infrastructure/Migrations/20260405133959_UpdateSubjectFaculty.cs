using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubjectFaculty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblSubjectFaculty_SubjectSemesterId_DivisionId",
                table: "TblSubjectFaculty");

            migrationBuilder.DropIndex(
                name: "IX_TblExternalFacultyPermission_FacultyId",
                table: "TblExternalFacultyPermission");

            migrationBuilder.AddColumn<byte>(
                name: "TeachingType",
                table: "TblSubjectFaculty",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_SubjectSemesterId_DivisionId_TeachingType",
                table: "TblSubjectFaculty",
                columns: new[] { "SubjectSemesterId", "DivisionId", "TeachingType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblExternalFacultyPermission_FacultyId_DepartmentId",
                table: "TblExternalFacultyPermission",
                columns: new[] { "FacultyId", "DepartmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblSubjectFaculty_SubjectSemesterId_DivisionId_TeachingType",
                table: "TblSubjectFaculty");

            migrationBuilder.DropIndex(
                name: "IX_TblExternalFacultyPermission_FacultyId_DepartmentId",
                table: "TblExternalFacultyPermission");

            migrationBuilder.DropColumn(
                name: "TeachingType",
                table: "TblSubjectFaculty");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_SubjectSemesterId_DivisionId",
                table: "TblSubjectFaculty",
                columns: new[] { "SubjectSemesterId", "DivisionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblExternalFacultyPermission_FacultyId",
                table: "TblExternalFacultyPermission",
                column: "FacultyId");
        }
    }
}
