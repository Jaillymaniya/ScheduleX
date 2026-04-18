using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubjectOfferingFromEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblTimeTableEntry_SubjectOffering_OfferingId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropTable(
                name: "SubjectOffering");

            migrationBuilder.RenameColumn(
                name: "OfferingId",
                table: "TblTimeTableEntry",
                newName: "SubjectSemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_TblTimeTableEntry_OfferingId",
                table: "TblTimeTableEntry",
                newName: "IX_TblTimeTableEntry_SubjectSemesterId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TblTimeTableEntry_TblSubjectSemester_SubjectSemesterId",
                table: "TblTimeTableEntry",
                column: "SubjectSemesterId",
                principalTable: "TblSubjectSemester",
                principalColumn: "SubjectSemesterId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblTimeTableEntry_TblFaculty_FacultyId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_TblTimeTableEntry_TblSubjectSemester_SubjectSemesterId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropIndex(
                name: "IX_TblTimeTableEntry_FacultyId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "TblTimeTableEntry");

            migrationBuilder.RenameColumn(
                name: "SubjectSemesterId",
                table: "TblTimeTableEntry",
                newName: "OfferingId");

            migrationBuilder.RenameIndex(
                name: "IX_TblTimeTableEntry_SubjectSemesterId",
                table: "TblTimeTableEntry",
                newName: "IX_TblTimeTableEntry_OfferingId");

            migrationBuilder.CreateTable(
                name: "SubjectOffering",
                columns: table => new
                {
                    OfferingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyId = table.Column<int>(type: "int", nullable: false),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PracticalBlockSize = table.Column<byte>(type: "tinyint", nullable: true),
                    PracticalLecturesPerWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    PreferredRoomType = table.Column<byte>(type: "tinyint", nullable: true),
                    TheoryLecturesPerWeek = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectOffering", x => x.OfferingId);
                    table.ForeignKey(
                        name: "FK_SubjectOffering_TblFaculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "TblFaculty",
                        principalColumn: "FacultyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectOffering_TblSemester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "TblSemester",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectOffering_TblSubject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "TblSubject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectOffering_FacultyId",
                table: "SubjectOffering",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectOffering_SemesterId",
                table: "SubjectOffering",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectOffering_SubjectId",
                table: "SubjectOffering",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblTimeTableEntry_SubjectOffering_OfferingId",
                table: "TblTimeTableEntry",
                column: "OfferingId",
                principalTable: "SubjectOffering",
                principalColumn: "OfferingId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
