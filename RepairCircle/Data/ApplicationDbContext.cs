using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data.Models;

namespace RepairCircle.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ToolCategory> ToolCategories => Set<ToolCategory>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Tool> Tools => Set<Tool>();
    public DbSet<RepairRequest> RepairRequests => Set<RepairRequest>();
    public DbSet<RepairSession> RepairSessions => Set<RepairSession>();
    public DbSet<VolunteerProfile> VolunteerProfiles => Set<VolunteerProfile>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ToolCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Entity<Skill>()
            .HasIndex(s => s.Name)
            .IsUnique();

        builder.Entity<BorrowRecord>()
            .HasIndex(b => b.BorrowReference)
            .IsUnique();

        builder.Entity<RepairRequest>()
            .HasIndex(r => r.RequestReference)
            .IsUnique();

        builder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.ToolId })
            .IsUnique();

        builder.Entity<Feedback>()
            .HasIndex(f => new { f.UserId, f.RepairRequestId })
            .IsUnique();

        builder.Entity<VolunteerProfile>()
            .HasIndex(v => v.UserId)
            .IsUnique();

        builder.Entity<Location>()
            .Property(l => l.Latitude)
            .HasColumnType("decimal(9,6)");

        builder.Entity<Location>()
            .Property(l => l.Longitude)
            .HasColumnType("decimal(9,6)");

        builder.Entity<Tool>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_Tools_Quantity_NonNegative", "[Quantity] >= 0");
            });

        builder.Entity<BorrowRecord>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_BorrowRecords_DueDate_After_BorrowDate", "[DueDate] >= [BorrowDate]");
            });

        builder.Entity<RepairSession>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_RepairSessions_AvailableSlots_Valid", "[AvailableSlots] >= 0 AND [AvailableSlots] <= [MaxParticipants]");
                t.HasCheckConstraint("CK_RepairSessions_EndDate_After_StartDate", "[EndDate] >= [StartDate]");
            });

        builder.Entity<Tool>()
            .HasOne(t => t.ToolCategory)
            .WithMany(c => c.Tools)
            .HasForeignKey(t => t.ToolCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Tool>()
            .HasOne(t => t.Location)
            .WithMany(l => l.Tools)
            .HasForeignKey(t => t.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RepairRequest>()
            .HasOne(r => r.SubmittedByUser)
            .WithMany(u => u.RepairRequests)
            .HasForeignKey(r => r.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RepairRequest>()
            .HasOne(r => r.AssignedVolunteerProfile)
            .WithMany(v => v.AssignedRepairRequests)
            .HasForeignKey(r => r.AssignedVolunteerProfileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<RepairRequest>()
            .HasOne(r => r.Location)
            .WithMany(l => l.RepairRequests)
            .HasForeignKey(r => r.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RepairRequest>()
            .HasOne(r => r.RepairSession)
            .WithMany(rs => rs.RepairRequests)
            .HasForeignKey(r => r.RepairSessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<RepairSession>()
            .HasOne(rs => rs.Location)
            .WithMany(l => l.RepairSessions)
            .HasForeignKey(rs => rs.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VolunteerProfile>()
            .HasOne(v => v.User)
            .WithOne()
            .HasForeignKey<VolunteerProfile>(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VolunteerProfile>()
            .HasMany(v => v.Skills)
            .WithMany(s => s.VolunteerProfiles)
            .UsingEntity<Dictionary<string, object>>(
                "VolunteerProfileSkill",
                j => j
                    .HasOne<Skill>()
                    .WithMany()
                    .HasForeignKey("SkillId")
                    .HasConstraintName("FK_VolunteerProfileSkill_Skills_SkillId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<VolunteerProfile>()
                    .WithMany()
                    .HasForeignKey("VolunteerProfileId")
                    .HasConstraintName("FK_VolunteerProfileSkill_VolunteerProfiles_VolunteerProfileId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("VolunteerProfileSkills");
                    j.HasKey("VolunteerProfileId", "SkillId");
                });

        builder.Entity<VolunteerProfile>()
            .HasMany(v => v.RepairSessions)
            .WithMany(rs => rs.Volunteers)
            .UsingEntity<Dictionary<string, object>>(
                "RepairSessionVolunteer",
                j => j
                    .HasOne<RepairSession>()
                    .WithMany()
                    .HasForeignKey("RepairSessionId")
                    .HasConstraintName("FK_RepairSessionVolunteer_RepairSessions_RepairSessionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<VolunteerProfile>()
                    .WithMany()
                    .HasForeignKey("VolunteerProfileId")
                    .HasConstraintName("FK_RepairSessionVolunteer_VolunteerProfiles_VolunteerProfileId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("RepairSessionVolunteers");
                    j.HasKey("RepairSessionId", "VolunteerProfileId");
                });

        builder.Entity<BorrowRecord>()
            .HasOne(b => b.Tool)
            .WithMany(t => t.BorrowRecords)
            .HasForeignKey(b => b.ToolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BorrowRecord>()
            .HasOne(b => b.User)
            .WithMany(u => u.BorrowRecords)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Feedback>()
            .HasOne(f => f.RepairRequest)
            .WithMany(r => r.Feedbacks)
            .HasForeignKey(f => f.RepairRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Favorite>()
            .HasOne(f => f.Tool)
            .WithMany(t => t.Favorites)
            .HasForeignKey(f => f.ToolId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
