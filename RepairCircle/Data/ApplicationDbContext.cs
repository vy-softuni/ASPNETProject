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

        builder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.ToolId })
            .IsUnique();

        builder.Entity<Location>()
            .Property(l => l.Latitude)
            .HasColumnType("decimal(9,6)");

        builder.Entity<Location>()
            .Property(l => l.Longitude)
            .HasColumnType("decimal(9,6)");

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
