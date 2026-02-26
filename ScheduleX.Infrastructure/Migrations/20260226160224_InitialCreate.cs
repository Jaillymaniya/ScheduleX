using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblDepartment",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDepartment", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "TblTimeTableTemplate",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LayoutType = table.Column<byte>(type: "tinyint", nullable: false),
                    TemplateJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeTableTemplate", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "TblCourse",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCourse", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_TblCourse_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblFaculty",
                columns: table => new
                {
                    FacultyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    FacultyName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FacultyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaxLecturesPerDay = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblFaculty", x => x.FacultyId);
                    table.ForeignKey(
                        name: "FK_TblFaculty_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblRoom",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoomType = table.Column<byte>(type: "tinyint", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRoom", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_TblRoom_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblSubject",
                columns: table => new
                {
                    SubjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Credits = table.Column<byte>(type: "tinyint", nullable: false),
                    SubjectCategory = table.Column<byte>(type: "tinyint", nullable: false),
                    IsElective = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSubject", x => x.SubjectId);
                    table.ForeignKey(
                        name: "FK_TblSubject_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<byte>(type: "tinyint", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblUser", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_TblUser_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblScheduleConfig",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    LectureDurationMin = table.Column<int>(type: "int", nullable: false),
                    WorkingDaysMask = table.Column<int>(type: "int", nullable: false),
                    LecturesPerDay = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblScheduleConfig", x => x.ConfigId);
                    table.ForeignKey(
                        name: "FK_TblScheduleConfig_TblCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "TblCourse",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblScheduleConfig_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblSemester",
                columns: table => new
                {
                    SemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    SemesterNo = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSemester", x => x.SemesterId);
                    table.ForeignKey(
                        name: "FK_TblSemester_TblCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "TblCourse",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblFacultyAvailability",
                columns: table => new
                {
                    AvailabilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblFacultyAvailability", x => x.AvailabilityId);
                    table.ForeignKey(
                        name: "FK_TblFacultyAvailability_TblFaculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "TblFaculty",
                        principalColumn: "FacultyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblBreakRule",
                columns: table => new
                {
                    BreakRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    BreakNo = table.Column<byte>(type: "tinyint", nullable: false),
                    AfterLectureNo = table.Column<byte>(type: "tinyint", nullable: false),
                    BreakDurationMin = table.Column<int>(type: "int", nullable: false),
                    BreakName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBreakRule", x => x.BreakRuleId);
                    table.ForeignKey(
                        name: "FK_TblBreakRule_TblScheduleConfig_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "TblScheduleConfig",
                        principalColumn: "ConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblTimeTableBatch",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    VersionNo = table.Column<int>(type: "int", nullable: false),
                    ParentBatchId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActiveVersion = table.Column<bool>(type: "bit", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeTableBatch", x => x.BatchId);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatch_TblCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "TblCourse",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatch_TblDepartment_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TblDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatch_TblScheduleConfig_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "TblScheduleConfig",
                        principalColumn: "ConfigId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatch_TblTimeTableBatch_ParentBatchId",
                        column: x => x.ParentBatchId,
                        principalTable: "TblTimeTableBatch",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatch_TblTimeTableTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "TblTimeTableTemplate",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatch_TblUser_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "TblUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblDivision",
                columns: table => new
                {
                    DivisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    DivisionName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StudentStrength = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDivision", x => x.DivisionId);
                    table.ForeignKey(
                        name: "FK_TblDivision_TblSemester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "TblSemester",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblSubjectOffering",
                columns: table => new
                {
                    OfferingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    FacultyId = table.Column<int>(type: "int", nullable: false),
                    TheoryLecturesPerWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    PracticalLecturesPerWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    PracticalBlockSize = table.Column<byte>(type: "tinyint", nullable: true),
                    PreferredRoomType = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSubjectOffering", x => x.OfferingId);
                    table.ForeignKey(
                        name: "FK_TblSubjectOffering_TblFaculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "TblFaculty",
                        principalColumn: "FacultyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblSubjectOffering_TblSemester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "TblSemester",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblSubjectOffering_TblSubject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "TblSubject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblTimeSlot",
                columns: table => new
                {
                    TimeSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    SlotNo = table.Column<byte>(type: "tinyint", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    SlotType = table.Column<byte>(type: "tinyint", nullable: false),
                    BreakRuleId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeSlot", x => x.TimeSlotId);
                    table.ForeignKey(
                        name: "FK_TblTimeSlot_TblBreakRule_BreakRuleId",
                        column: x => x.BreakRuleId,
                        principalTable: "TblBreakRule",
                        principalColumn: "BreakRuleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeSlot_TblScheduleConfig_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "TblScheduleConfig",
                        principalColumn: "ConfigId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblBatchTemplateSnapshot",
                columns: table => new
                {
                    SnapshotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    TemplateJsonSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBatchTemplateSnapshot", x => x.SnapshotId);
                    table.ForeignKey(
                        name: "FK_TblBatchTemplateSnapshot_TblTimeTableBatch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "TblTimeTableBatch",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblBatchTemplateSnapshot_TblTimeTableTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "TblTimeTableTemplate",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblTimeTableBatchSemester",
                columns: table => new
                {
                    BatchSemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeTableBatchSemester", x => x.BatchSemesterId);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatchSemester_TblSemester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "TblSemester",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableBatchSemester_TblTimeTableBatch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "TblTimeTableBatch",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblDivisionRoomAllocation",
                columns: table => new
                {
                    AllocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    DivisionId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDivisionRoomAllocation", x => x.AllocationId);
                    table.ForeignKey(
                        name: "FK_TblDivisionRoomAllocation_TblDivision_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "TblDivision",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblDivisionRoomAllocation_TblRoom_RoomId",
                        column: x => x.RoomId,
                        principalTable: "TblRoom",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblDivisionRoomAllocation_TblSemester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "TblSemester",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblTimeTableEntry",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    DivisionId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    TimeSlotId = table.Column<int>(type: "int", nullable: false),
                    EntryType = table.Column<byte>(type: "tinyint", nullable: false),
                    OfferingId = table.Column<int>(type: "int", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BlockPart = table.Column<byte>(type: "tinyint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeTableEntry", x => x.EntryId);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntry_TblDivision_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "TblDivision",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntry_TblRoom_RoomId",
                        column: x => x.RoomId,
                        principalTable: "TblRoom",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntry_TblSemester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "TblSemester",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntry_TblSubjectOffering_OfferingId",
                        column: x => x.OfferingId,
                        principalTable: "TblSubjectOffering",
                        principalColumn: "OfferingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntry_TblTimeSlot_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TblTimeSlot",
                        principalColumn: "TimeSlotId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntry_TblTimeTableBatch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "TblTimeTableBatch",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblExportHistory",
                columns: table => new
                {
                    ExportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    ExportType = table.Column<byte>(type: "tinyint", nullable: false),
                    TemplateSnapshotId = table.Column<int>(type: "int", nullable: false),
                    ExportedByUserId = table.Column<int>(type: "int", nullable: false),
                    ExportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblExportHistory", x => x.ExportId);
                    table.ForeignKey(
                        name: "FK_TblExportHistory_TblBatchTemplateSnapshot_TemplateSnapshotId",
                        column: x => x.TemplateSnapshotId,
                        principalTable: "TblBatchTemplateSnapshot",
                        principalColumn: "SnapshotId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblExportHistory_TblTimeTableBatch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "TblTimeTableBatch",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblExportHistory_TblUser_ExportedByUserId",
                        column: x => x.ExportedByUserId,
                        principalTable: "TblUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblTimeTableEntryHistory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OldDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeTableEntryHistory", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntryHistory_TblTimeTableBatch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "TblTimeTableBatch",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntryHistory_TblTimeTableEntry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "TblTimeTableEntry",
                        principalColumn: "EntryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTimeTableEntryHistory_TblUser_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "TblUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblBatchTemplateSnapshot_BatchId",
                table: "TblBatchTemplateSnapshot",
                column: "BatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblBatchTemplateSnapshot_TemplateId",
                table: "TblBatchTemplateSnapshot",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TblBreakRule_ConfigId_AfterLectureNo",
                table: "TblBreakRule",
                columns: new[] { "ConfigId", "AfterLectureNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblBreakRule_ConfigId_BreakNo",
                table: "TblBreakRule",
                columns: new[] { "ConfigId", "BreakNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblCourse_CourseCode",
                table: "TblCourse",
                column: "CourseCode",
                unique: true,
                filter: "[CourseCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblCourse_DepartmentId",
                table: "TblCourse",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblDepartment_DepartmentCode",
                table: "TblDepartment",
                column: "DepartmentCode",
                unique: true,
                filter: "[DepartmentCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblDepartment_DepartmentName",
                table: "TblDepartment",
                column: "DepartmentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblDivision_SemesterId_DivisionName",
                table: "TblDivision",
                columns: new[] { "SemesterId", "DivisionName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_DivisionId",
                table: "TblDivisionRoomAllocation",
                column: "DivisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_RoomId",
                table: "TblDivisionRoomAllocation",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_SemesterId",
                table: "TblDivisionRoomAllocation",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TblExportHistory_BatchId",
                table: "TblExportHistory",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblExportHistory_ExportedByUserId",
                table: "TblExportHistory",
                column: "ExportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblExportHistory_TemplateSnapshotId",
                table: "TblExportHistory",
                column: "TemplateSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_TblFaculty_DepartmentId",
                table: "TblFaculty",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblFaculty_FacultyCode",
                table: "TblFaculty",
                column: "FacultyCode",
                unique: true,
                filter: "[FacultyCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblFacultyAvailability_FacultyId",
                table: "TblFacultyAvailability",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_TblRoom_DepartmentId",
                table: "TblRoom",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblRoom_RoomName",
                table: "TblRoom",
                column: "RoomName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblScheduleConfig_CourseId",
                table: "TblScheduleConfig",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TblScheduleConfig_DepartmentId",
                table: "TblScheduleConfig",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSemester_CourseId_SemesterNo",
                table: "TblSemester",
                columns: new[] { "CourseId", "SemesterNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubject_DepartmentId",
                table: "TblSubject",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubject_SubjectCode",
                table: "TblSubject",
                column: "SubjectCode",
                unique: true,
                filter: "[SubjectCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectOffering_FacultyId",
                table: "TblSubjectOffering",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectOffering_SemesterId_SubjectId",
                table: "TblSubjectOffering",
                columns: new[] { "SemesterId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectOffering_SubjectId",
                table: "TblSubjectOffering",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeSlot_BreakRuleId",
                table: "TblTimeSlot",
                column: "BreakRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeSlot_ConfigId_SlotNo",
                table: "TblTimeSlot",
                columns: new[] { "ConfigId", "SlotNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatch_ConfigId",
                table: "TblTimeTableBatch",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatch_CourseId",
                table: "TblTimeTableBatch",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatch_CreatedByUserId",
                table: "TblTimeTableBatch",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatch_DepartmentId",
                table: "TblTimeTableBatch",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatch_ParentBatchId",
                table: "TblTimeTableBatch",
                column: "ParentBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatch_TemplateId",
                table: "TblTimeTableBatch",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatchSemester_BatchId_SemesterId",
                table: "TblTimeTableBatchSemester",
                columns: new[] { "BatchId", "SemesterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatchSemester_SemesterId",
                table: "TblTimeTableBatchSemester",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_BatchId_DivisionId_DayOfWeek_TimeSlotId",
                table: "TblTimeTableEntry",
                columns: new[] { "BatchId", "DivisionId", "DayOfWeek", "TimeSlotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_BatchId_RoomId_DayOfWeek_TimeSlotId",
                table: "TblTimeTableEntry",
                columns: new[] { "BatchId", "RoomId", "DayOfWeek", "TimeSlotId" },
                unique: true,
                filter: "[RoomId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_DivisionId",
                table: "TblTimeTableEntry",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_OfferingId",
                table: "TblTimeTableEntry",
                column: "OfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_RoomId",
                table: "TblTimeTableEntry",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_SemesterId",
                table: "TblTimeTableEntry",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntry_TimeSlotId",
                table: "TblTimeTableEntry",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntryHistory_BatchId",
                table: "TblTimeTableEntryHistory",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntryHistory_ChangedByUserId",
                table: "TblTimeTableEntryHistory",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableEntryHistory_EntryId",
                table: "TblTimeTableEntryHistory",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableTemplate_TemplateName",
                table: "TblTimeTableTemplate",
                column: "TemplateName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblUser_DepartmentId",
                table: "TblUser",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblUser_Username",
                table: "TblUser",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblDivisionRoomAllocation");

            migrationBuilder.DropTable(
                name: "TblExportHistory");

            migrationBuilder.DropTable(
                name: "TblFacultyAvailability");

            migrationBuilder.DropTable(
                name: "TblTimeTableBatchSemester");

            migrationBuilder.DropTable(
                name: "TblTimeTableEntryHistory");

            migrationBuilder.DropTable(
                name: "TblBatchTemplateSnapshot");

            migrationBuilder.DropTable(
                name: "TblTimeTableEntry");

            migrationBuilder.DropTable(
                name: "TblDivision");

            migrationBuilder.DropTable(
                name: "TblRoom");

            migrationBuilder.DropTable(
                name: "TblSubjectOffering");

            migrationBuilder.DropTable(
                name: "TblTimeSlot");

            migrationBuilder.DropTable(
                name: "TblTimeTableBatch");

            migrationBuilder.DropTable(
                name: "TblFaculty");

            migrationBuilder.DropTable(
                name: "TblSemester");

            migrationBuilder.DropTable(
                name: "TblSubject");

            migrationBuilder.DropTable(
                name: "TblBreakRule");

            migrationBuilder.DropTable(
                name: "TblTimeTableTemplate");

            migrationBuilder.DropTable(
                name: "TblUser");

            migrationBuilder.DropTable(
                name: "TblScheduleConfig");

            migrationBuilder.DropTable(
                name: "TblCourse");

            migrationBuilder.DropTable(
                name: "TblDepartment");
        }
    }
}
