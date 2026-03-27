using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitSubjectOffering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectOffering_TblFaculty_FacultyId",
                table: "TblSubjectOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectOffering_TblSemester_SemesterId",
                table: "TblSubjectOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectOffering_TblSubject_SubjectId",
                table: "TblSubjectOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_TblTimeTableEntry_TblSubjectOffering_OfferingId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblSubjectOffering",
                table: "TblSubjectOffering");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectOffering_SemesterId_SubjectId",
                table: "TblSubjectOffering");

            migrationBuilder.RenameTable(
                name: "TblSubjectOffering",
                newName: "SubjectOffering");

            migrationBuilder.RenameIndex(
                name: "IX_TblSubjectOffering_SubjectId",
                table: "SubjectOffering",
                newName: "IX_SubjectOffering_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TblSubjectOffering_FacultyId",
                table: "SubjectOffering",
                newName: "IX_SubjectOffering_FacultyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectOffering",
                table: "SubjectOffering",
                column: "OfferingId");

            migrationBuilder.CreateTable(
                name: "TblSubjectFaculty",
                columns: table => new
                {
                    SubjectFacultyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    FacultyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSubjectFaculty", x => x.SubjectFacultyId);
                    table.ForeignKey(
                        name: "FK_TblSubjectFaculty_TblFaculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "TblFaculty",
                        principalColumn: "FacultyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblSubjectFaculty_TblSubject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "TblSubject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblSubjectLectureConfig",
                columns: table => new
                {
                    SubjectLectureConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TheoryLecturesPerWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    PracticalLecturesPerWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    PracticalBlockSize = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSubjectLectureConfig", x => x.SubjectLectureConfigId);
                    table.ForeignKey(
                        name: "FK_TblSubjectLectureConfig_TblSubject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "TblSubject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblSubjectRoomConfig",
                columns: table => new
                {
                    SubjectRoomConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    PreferredRoomType = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSubjectRoomConfig", x => x.SubjectRoomConfigId);
                    table.ForeignKey(
                        name: "FK_TblSubjectRoomConfig_TblRoom_RoomId",
                        column: x => x.RoomId,
                        principalTable: "TblRoom",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblSubjectRoomConfig_TblSubject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "TblSubject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblSubjectSemester",
                columns: table => new
                {
                    SubjectSemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSubjectSemester", x => x.SubjectSemesterId);
                    table.ForeignKey(
                        name: "FK_TblSubjectSemester_TblSemester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "TblSemester",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblSubjectSemester_TblSubject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "TblSubject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectOffering_SemesterId",
                table: "SubjectOffering",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_FacultyId",
                table: "TblSubjectFaculty",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectFaculty_SubjectId_FacultyId",
                table: "TblSubjectFaculty",
                columns: new[] { "SubjectId", "FacultyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectLectureConfig_SubjectId",
                table: "TblSubjectLectureConfig",
                column: "SubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectRoomConfig_RoomId",
                table: "TblSubjectRoomConfig",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectRoomConfig_SubjectId",
                table: "TblSubjectRoomConfig",
                column: "SubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectSemester_SemesterId",
                table: "TblSubjectSemester",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectSemester_SubjectId_SemesterId",
                table: "TblSubjectSemester",
                columns: new[] { "SubjectId", "SemesterId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectOffering_TblFaculty_FacultyId",
                table: "SubjectOffering",
                column: "FacultyId",
                principalTable: "TblFaculty",
                principalColumn: "FacultyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectOffering_TblSemester_SemesterId",
                table: "SubjectOffering",
                column: "SemesterId",
                principalTable: "TblSemester",
                principalColumn: "SemesterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectOffering_TblSubject_SubjectId",
                table: "SubjectOffering",
                column: "SubjectId",
                principalTable: "TblSubject",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblTimeTableEntry_SubjectOffering_OfferingId",
                table: "TblTimeTableEntry",
                column: "OfferingId",
                principalTable: "SubjectOffering",
                principalColumn: "OfferingId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectOffering_TblFaculty_FacultyId",
                table: "SubjectOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectOffering_TblSemester_SemesterId",
                table: "SubjectOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectOffering_TblSubject_SubjectId",
                table: "SubjectOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_TblTimeTableEntry_SubjectOffering_OfferingId",
                table: "TblTimeTableEntry");

            migrationBuilder.DropTable(
                name: "TblSubjectFaculty");

            migrationBuilder.DropTable(
                name: "TblSubjectLectureConfig");

            migrationBuilder.DropTable(
                name: "TblSubjectRoomConfig");

            migrationBuilder.DropTable(
                name: "TblSubjectSemester");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectOffering",
                table: "SubjectOffering");

            migrationBuilder.DropIndex(
                name: "IX_SubjectOffering_SemesterId",
                table: "SubjectOffering");

            migrationBuilder.RenameTable(
                name: "SubjectOffering",
                newName: "TblSubjectOffering");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectOffering_SubjectId",
                table: "TblSubjectOffering",
                newName: "IX_TblSubjectOffering_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectOffering_FacultyId",
                table: "TblSubjectOffering",
                newName: "IX_TblSubjectOffering_FacultyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblSubjectOffering",
                table: "TblSubjectOffering",
                column: "OfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectOffering_SemesterId_SubjectId",
                table: "TblSubjectOffering",
                columns: new[] { "SemesterId", "SubjectId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectOffering_TblFaculty_FacultyId",
                table: "TblSubjectOffering",
                column: "FacultyId",
                principalTable: "TblFaculty",
                principalColumn: "FacultyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectOffering_TblSemester_SemesterId",
                table: "TblSubjectOffering",
                column: "SemesterId",
                principalTable: "TblSemester",
                principalColumn: "SemesterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectOffering_TblSubject_SubjectId",
                table: "TblSubjectOffering",
                column: "SubjectId",
                principalTable: "TblSubject",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblTimeTableEntry_TblSubjectOffering_OfferingId",
                table: "TblTimeTableEntry",
                column: "OfferingId",
                principalTable: "TblSubjectOffering",
                principalColumn: "OfferingId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
