using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;

namespace ScheduleX.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets (tables)
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<User> Users => Set<User>();
    //public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Division> Divisions => Set<Division>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<DivisionRoomAllocation> DivisionRoomAllocations => Set<DivisionRoomAllocation>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<FacultyAvailability> FacultyAvailabilities => Set<FacultyAvailability>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SubjectOffering> SubjectOfferings => Set<SubjectOffering>();
    public DbSet<ScheduleConfig> ScheduleConfigs => Set<ScheduleConfig>();
    public DbSet<BreakRule> BreakRules => Set<BreakRule>();
    public DbSet<TimeTableTemplate> TimeTableTemplates => Set<TimeTableTemplate>();
    public DbSet<TimeTableBatch> TimeTableBatches => Set<TimeTableBatch>();
    public DbSet<TimeTableBatchSemester> TimeTableBatchSemesters => Set<TimeTableBatchSemester>();
    public DbSet<BatchTemplateSnapshot> BatchTemplateSnapshots => Set<BatchTemplateSnapshot>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<TimeTableEntry> TimeTableEntries => Set<TimeTableEntry>();
    public DbSet<TimeTableEntryHistory> TimeTableEntryHistories => Set<TimeTableEntryHistory>();
    public DbSet<ExportHistory> ExportHistories => Set<ExportHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Table names exactly like your DB dictionary
        modelBuilder.Entity<Department>().ToTable("TblDepartment");
        modelBuilder.Entity<User>().ToTable("TblUser");
        //modelBuilder.Entity<PasswordResetToken>().ToTable("TblPasswordResetToken");
        modelBuilder.Entity<Course>().ToTable("TblCourse");
        modelBuilder.Entity<Semester>().ToTable("TblSemester");
        modelBuilder.Entity<Division>().ToTable("TblDivision");
        modelBuilder.Entity<Room>().ToTable("TblRoom");
        modelBuilder.Entity<DivisionRoomAllocation>().ToTable("TblDivisionRoomAllocation");
        modelBuilder.Entity<Faculty>().ToTable("TblFaculty");
        modelBuilder.Entity<FacultyAvailability>().ToTable("TblFacultyAvailability");
        modelBuilder.Entity<Subject>().ToTable("TblSubject");
        modelBuilder.Entity<SubjectOffering>().ToTable("TblSubjectOffering");
        modelBuilder.Entity<ScheduleConfig>().ToTable("TblScheduleConfig");
        modelBuilder.Entity<BreakRule>().ToTable("TblBreakRule");
        modelBuilder.Entity<TimeTableTemplate>().ToTable("TblTimeTableTemplate");
        modelBuilder.Entity<TimeTableBatch>().ToTable("TblTimeTableBatch");
        modelBuilder.Entity<TimeTableBatchSemester>().ToTable("TblTimeTableBatchSemester");
        modelBuilder.Entity<BatchTemplateSnapshot>().ToTable("TblBatchTemplateSnapshot");
        modelBuilder.Entity<TimeSlot>().ToTable("TblTimeSlot");
        modelBuilder.Entity<TimeTableEntry>().ToTable("TblTimeTableEntry");
        modelBuilder.Entity<TimeTableEntryHistory>().ToTable("TblTimeTableEntryHistory");
        modelBuilder.Entity<ExportHistory>().ToTable("TblExportHistory");

        // ✅ UNIQUE constraints / indexes
        modelBuilder.Entity<Department>()
            .HasIndex(x => x.DepartmentName).IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(x => x.DepartmentCode).IsUnique()
            .HasFilter("[DepartmentCode] IS NOT NULL");

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Username).IsUnique();

        //modelBuilder.Entity<PasswordResetToken>()
        //    .HasIndex(x => x.Token).IsUnique();

        modelBuilder.Entity<Course>()
            .HasIndex(x => x.CourseCode).IsUnique()
            .HasFilter("[CourseCode] IS NOT NULL");

        modelBuilder.Entity<Semester>()
            .HasIndex(x => new { x.CourseId, x.SemesterNo }).IsUnique();

        modelBuilder.Entity<Division>()
            .HasIndex(x => new { x.SemesterId, x.DivisionName }).IsUnique();

        modelBuilder.Entity<Room>()
            .HasIndex(x => x.RoomName).IsUnique();

        // one fixed base room per division (your constraint UNIQUE(DivisionId))
        modelBuilder.Entity<DivisionRoomAllocation>()
            .HasIndex(x => x.DivisionId).IsUnique();

        modelBuilder.Entity<Faculty>()
            .HasIndex(x => x.FacultyCode).IsUnique()
            .HasFilter("[FacultyCode] IS NOT NULL");

        modelBuilder.Entity<Subject>()
            .HasIndex(x => x.SubjectCode).IsUnique()
            .HasFilter("[SubjectCode] IS NOT NULL");

        modelBuilder.Entity<SubjectOffering>()
            .HasIndex(x => new { x.SemesterId, x.SubjectId }).IsUnique();

        modelBuilder.Entity<BreakRule>()
            .HasIndex(x => new { x.ConfigId, x.BreakNo }).IsUnique();

        modelBuilder.Entity<BreakRule>()
            .HasIndex(x => new { x.ConfigId, x.AfterLectureNo }).IsUnique();

        modelBuilder.Entity<TimeTableTemplate>()
            .HasIndex(x => x.TemplateName).IsUnique();

        modelBuilder.Entity<TimeTableBatchSemester>()
            .HasIndex(x => new { x.BatchId, x.SemesterId }).IsUnique();

        modelBuilder.Entity<BatchTemplateSnapshot>()
            .HasIndex(x => x.BatchId).IsUnique();

        modelBuilder.Entity<TimeSlot>()
            .HasIndex(x => new { x.ConfigId, x.SlotNo }).IsUnique();

        // Prevent duplicate entries in same division slot
        modelBuilder.Entity<TimeTableEntry>()
            .HasIndex(x => new { x.BatchId, x.DivisionId, x.DayOfWeek, x.TimeSlotId })
            .IsUnique();

        // Room clash prevention (filtered unique index)
        modelBuilder.Entity<TimeTableEntry>()
            .HasIndex(x => new { x.BatchId, x.RoomId, x.DayOfWeek, x.TimeSlotId })
            .IsUnique()
            .HasFilter("[RoomId] IS NOT NULL");

        // ✅ Avoid multiple cascade paths (safe defaults)
        modelBuilder.Entity<User>()
            .HasOne(x => x.Department)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeTableBatch>()
            .HasOne(x => x.ParentBatch)
            .WithMany()
            .HasForeignKey(x => x.ParentBatchId)
            .OnDelete(DeleteBehavior.Restrict);


        // ===============================
        // ✅ PREVENT MULTIPLE CASCADE PATH
        // ===============================

        // Set ALL relationships to Restrict by default
        foreach (var fk in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // If you want cascade only for pure child tables,
        // explicitly allow them here:

        modelBuilder.Entity<FacultyAvailability>()
            .HasOne(x => x.Faculty)
            .WithMany(x => x.FacultyAvailabilities)
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BreakRule>()
            .HasOne(x => x.ScheduleConfig)
            .WithMany(x => x.BreakRules)
            .HasForeignKey(x => x.ConfigId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimeTableBatch>()
            .HasOne(x => x.ParentBatch)
            .WithMany()
            .HasForeignKey(x => x.ParentBatchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}