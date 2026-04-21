using Microsoft.EntityFrameworkCore;

namespace RepairCircle.Tests;

internal static class TestDbFactory
{
    public static ApplicationDbContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static async Task SeedBasicAsync(ApplicationDbContext db)
    {
        if (await db.ToolCategories.AnyAsync())
        {
            return;
        }

        var user = new ApplicationUser
        {
            Id = "user-1",
            UserName = "user1",
            Email = "user1@example.com",
            FullName = "User One"
        };

        var otherUser = new ApplicationUser
        {
            Id = "user-2",
            UserName = "user2",
            Email = "user2@example.com",
            FullName = "User Two"
        };

        var volunteerUser = new ApplicationUser
        {
            Id = "vol-1",
            UserName = "volunteer1",
            Email = "vol1@example.com",
            FullName = "Volunteer One"
        };

        var location1 = new Location
        {
            Id = 1,
            Name = "Central Hub",
            Address = "1 Main St",
            City = "Sofia",
            Latitude = 42.6977m,
            Longitude = 23.3219m
        };

        var location2 = new Location
        {
            Id = 2,
            Name = "North Hub",
            Address = "2 North St",
            City = "Varna"
        };

        var category1 = new ToolCategory { Id = 1, Name = "Electronics" };
        var category2 = new ToolCategory { Id = 2, Name = "Bike" };

        var skill1 = new Skill { Id = 1, Name = "Soldering" };
        var skill2 = new Skill { Id = 2, Name = "Bike Repair" };

        var volunteer = new VolunteerProfile
        {
            Id = 1,
            UserId = volunteerUser.Id,
            User = volunteerUser,
            Bio = "Experienced fixer",
            ExperienceLevel = ExperienceLevel.Advanced,
            IsApproved = true,
            Skills = new List<Skill> { skill1, skill2 }
        };

        var session1 = new RepairSession
        {
            Id = 1,
            Title = "Saturday Repair Cafe",
            Description = "Bring your electronics.",
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(3).AddHours(4),
            MaxParticipants = 12,
            AvailableSlots = 6,
            LocationId = location1.Id,
            Location = location1,
            Volunteers = new List<VolunteerProfile> { volunteer }
        };

        var sessionPast = new RepairSession
        {
            Id = 2,
            Title = "Old Session",
            Description = "Past one",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(-10).AddHours(2),
            MaxParticipants = 10,
            AvailableSlots = 0,
            LocationId = location2.Id,
            Location = location2
        };

        var tool1 = new Tool
        {
            Id = 1,
            Name = "Cordless Drill",
            Description = "Useful for many tasks",
            Condition = ToolCondition.Good,
            IsAvailable = true,
            Quantity = 2,
            ToolCategoryId = category1.Id,
            ToolCategory = category1,
            LocationId = location1.Id,
            Location = location1
        };

        var tool2 = new Tool
        {
            Id = 2,
            Name = "Bike Pump",
            Description = "For bikes",
            Condition = ToolCondition.Excellent,
            IsAvailable = false,
            Quantity = 0,
            ToolCategoryId = category2.Id,
            ToolCategory = category2,
            LocationId = location2.Id,
            Location = location2
        };

        var request1 = new RepairRequest
        {
            Id = 1,
            Title = "Broken Speaker",
            Description = "Does not power on",
            ItemType = "Speaker",
            RequestReference = "REQ-001",
            SubmittedByUserId = user.Id,
            SubmittedByUser = user,
            AssignedVolunteerProfileId = volunteer.Id,
            AssignedVolunteerProfile = volunteer,
            LocationId = location1.Id,
            Location = location1,
            RepairSessionId = session1.Id,
            RepairSession = session1,
            Status = RepairRequestStatus.Submitted,
            RequestedDate = DateTime.UtcNow.AddDays(-2)
        };

        var request2 = new RepairRequest
        {
            Id = 2,
            Title = "Jacket zipper issue",
            Description = "Zip stuck",
            ItemType = "Clothing",
            RequestReference = "REQ-002",
            SubmittedByUserId = otherUser.Id,
            SubmittedByUser = otherUser,
            LocationId = location2.Id,
            Location = location2,
            Status = RepairRequestStatus.Approved,
            RequestedDate = DateTime.UtcNow.AddDays(-1)
        };

        var borrow1 = new BorrowRecord
        {
            Id = 1,
            ToolId = tool1.Id,
            Tool = tool1,
            UserId = user.Id,
            User = user,
            BorrowDate = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(6),
            Status = BorrowStatus.Pending,
            BorrowReference = "BOR-001"
        };

        var feedback1 = new Feedback
        {
            Id = 1,
            UserId = user.Id,
            User = user,
            RepairRequestId = request1.Id,
            RepairRequest = request1,
            Rating = 5,
            Comment = "Very helpful",
            CreatedOn = DateTime.UtcNow.AddHours(-1)
        };

        var favorite1 = new Favorite
        {
            Id = 1,
            UserId = user.Id,
            User = user,
            ToolId = tool1.Id,
            Tool = tool1,
            CreatedOn = DateTime.UtcNow.AddMinutes(-30)
        };

        db.Users.AddRange(user, otherUser, volunteerUser);
        db.ToolCategories.AddRange(category1, category2);
        db.Locations.AddRange(location1, location2);
        db.Skills.AddRange(skill1, skill2);
        db.VolunteerProfiles.Add(volunteer);
        db.RepairSessions.AddRange(session1, sessionPast);
        db.Tools.AddRange(tool1, tool2);
        db.RepairRequests.AddRange(request1, request2);
        db.BorrowRecords.Add(borrow1);
        db.Feedbacks.Add(feedback1);
        db.Favorites.Add(favorite1);

        await db.SaveChangesAsync();
    }
}
