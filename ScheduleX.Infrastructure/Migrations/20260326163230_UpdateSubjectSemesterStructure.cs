using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubjectSemesterStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblSubjectRoomConfig_SubjectId",
                table: "TblSubjectRoomConfig");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectLectureConfig_SubjectId",
                table: "TblSubjectLectureConfig");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectFaculty_SubjectId_FacultyId",
                table: "TblSubjectFaculty");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TblSubjectRoomConfig",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SubjectSemesterId",
                table: "TblSubjectRoomConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TblSubjectLectureConfig",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SubjectSemesterId",
                table: "TblSubjectLectureConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TblSubjectFaculty",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "TblSubjectFaculty",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectSemesterId",
                table: "TblSubjectFaculty",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectRoomConfig_SubjectId",
                table: "TblSubjectRoomConfig",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectRoomConfig_SubjectSemesterId",
                table: "TblSubjectRoomConfig",
                column: "SubjectSemesterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectLectureConfig_SubjectId",
                table: "TblSubjectLectureConfig",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectLectureConfig_SubjectSemesterId",
                table: "TblSubjectLectureConfig",
                column: "SubjectSemesterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_DivisionId",
                table: "TblSubjectFaculty",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_SubjectId",
                table: "TblSubjectFaculty",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_SubjectSemesterId_DivisionId",
                table: "TblSubjectFaculty",
                columns: new[] { "SubjectSemesterId", "DivisionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectFaculty_TblDivision_DivisionId",
                table: "TblSubjectFaculty",
                column: "DivisionId",
                principalTable: "TblDivision",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectFaculty_TblSubjectSemester_SubjectSemesterId",
                table: "TblSubjectFaculty",
                column: "SubjectSemesterId",
                principalTable: "TblSubjectSemester",
                principalColumn: "SubjectSemesterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectLectureConfig_TblSubjectSemester_SubjectSemesterId",
                table: "TblSubjectLectureConfig",
                column: "SubjectSemesterId",
                principalTable: "TblSubjectSemester",
                principalColumn: "SubjectSemesterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectRoomConfig_TblSubjectSemester_SubjectSemesterId",
                table: "TblSubjectRoomConfig",
                column: "SubjectSemesterId",
                principalTable: "TblSubjectSemester",
                principalColumn: "SubjectSemesterId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectFaculty_TblDivision_DivisionId",
                table: "TblSubjectFaculty");

            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectFaculty_TblSubjectSemester_SubjectSemesterId",
                table: "TblSubjectFaculty");

            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectLectureConfig_TblSubjectSemester_SubjectSemesterId",
                table: "TblSubjectLectureConfig");

            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectRoomConfig_TblSubjectSemester_SubjectSemesterId",
                table: "TblSubjectRoomConfig");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectRoomConfig_SubjectId",
                table: "TblSubjectRoomConfig");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectRoomConfig_SubjectSemesterId",
                table: "TblSubjectRoomConfig");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectLectureConfig_SubjectId",
                table: "TblSubjectLectureConfig");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectLectureConfig_SubjectSemesterId",
                table: "TblSubjectLectureConfig");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectFaculty_DivisionId",
                table: "TblSubjectFaculty");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectFaculty_SubjectId",
                table: "TblSubjectFaculty");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectFaculty_SubjectSemesterId_DivisionId",
                table: "TblSubjectFaculty");

            migrationBuilder.DropColumn(
                name: "SubjectSemesterId",
                table: "TblSubjectRoomConfig");

            migrationBuilder.DropColumn(
                name: "SubjectSemesterId",
                table: "TblSubjectLectureConfig");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "TblSubjectFaculty");

            migrationBuilder.DropColumn(
                name: "SubjectSemesterId",
                table: "TblSubjectFaculty");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TblSubjectRoomConfig",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TblSubjectLectureConfig",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TblSubjectFaculty",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectRoomConfig_SubjectId",
                table: "TblSubjectRoomConfig",
                column: "SubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectLectureConfig_SubjectId",
                table: "TblSubjectLectureConfig",
                column: "SubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_SubjectId_FacultyId",
                table: "TblSubjectFaculty",
                columns: new[] { "SubjectId", "FacultyId" },
                unique: true);
        }
    }
}
