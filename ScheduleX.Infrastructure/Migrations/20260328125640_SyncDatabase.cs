//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace ScheduleX.Infrastructure.Migrations
//{
//    /// <inheritdoc />
//    public partial class SyncDatabase : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "ExternalFacultyPermissions",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    FacultyId = table.Column<int>(type: "int", nullable: false),
//                    DepartmentId = table.Column<int>(type: "int", nullable: false),
//                    IsActive = table.Column<bool>(type: "bit", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ExternalFacultyPermissions", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_ExternalFacultyPermissions_TblDepartment_DepartmentId",
//                        column: x => x.DepartmentId,
//                        principalTable: "TblDepartment",
//                        principalColumn: "DepartmentId",
//                        onDelete: ReferentialAction.Restrict);
//                    table.ForeignKey(
//                        name: "FK_ExternalFacultyPermissions_TblFaculty_FacultyId",
//                        column: x => x.FacultyId,
//                        principalTable: "TblFaculty",
//                        principalColumn: "FacultyId",
//                        onDelete: ReferentialAction.Restrict);
//                });

//            migrationBuilder.CreateIndex(
//                name: "IX_ExternalFacultyPermissions_DepartmentId",
//                table: "ExternalFacultyPermissions",
//                column: "DepartmentId");

//            migrationBuilder.CreateIndex(
//                name: "IX_ExternalFacultyPermissions_FacultyId",
//                table: "ExternalFacultyPermissions",
//                column: "FacultyId");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "ExternalFacultyPermissions");
//        }
//    }
//}







using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    public partial class SyncDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblExternalFacultyPermission",   // ✅ FIXED NAME
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    FacultyId = table.Column<int>(type: "int", nullable: false),

                    DepartmentId = table.Column<int>(type: "int", nullable: false),

                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblExternalFacultyPermission", x => x.Id);

                    table.ForeignKey(
                        name: "FK_TblExternalFacultyPermission_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_TblExternalFacultyPermission_TblFaculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "TblFaculty",
                        principalColumn: "FacultyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblExternalFacultyPermission_DepartmentId",
                table: "TblExternalFacultyPermission",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblExternalFacultyPermission_FacultyId",
                table: "TblExternalFacultyPermission",
                column: "FacultyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblExternalFacultyPermission");
        }
    }
}