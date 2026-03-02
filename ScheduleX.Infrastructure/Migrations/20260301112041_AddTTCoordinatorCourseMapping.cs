using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTTCoordinatorCourseMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblTTCoordinatorCourse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTTCoordinatorCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblTTCoordinatorCourse_TblCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "TblCourse",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTTCoordinatorCourse_TblUser_UserId",
                        column: x => x.UserId,
                        principalTable: "TblUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblTTCoordinatorCourse_CourseId",
                table: "TblTTCoordinatorCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTTCoordinatorCourse_UserId_CourseId",
                table: "TblTTCoordinatorCourse",
                columns: new[] { "UserId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblTTCoordinatorCourse");
        }
    }
}
