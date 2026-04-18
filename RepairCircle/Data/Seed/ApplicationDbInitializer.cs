using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data.Enums;
using RepairCircle.Data.Models;

namespace RepairCircle.Data.Seed;

public static class ApplicationDbInitializer
{
    public static async Task InitializeAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("ApplicationDbInitializer");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        try
        {
            if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                await dbContext.Database.MigrateAsync();
            }
            else
            {
                await dbContext.Database.EnsureCreatedAsync();
            }

            await SeedRolesAsync(roleManager);
            var seededUsers = await SeedUsersAsync(userManager);
            await SeedDomainDataAsync(dbContext, seededUsers, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[]
        {
            SeedConstants.AdministratorRoleName,
            SeedConstants.VolunteerRoleName,
            SeedConstants.UserRoleName
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                EnsureSucceeded(result, $"creating role '{role}'");
            }
        }
    }

    private static async Task<Dictionary<string, ApplicationUser>> SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var users = new List<(string Email, string Password, string FullName, string Role)>
        {
            (SeedConstants.AdminEmail, SeedConstants.AdminPassword, "System Administrator", SeedConstants.AdministratorRoleName),
            (SeedConstants.VolunteerOneEmail, SeedConstants.DefaultVolunteerPassword, "Maria Ivanova", SeedConstants.VolunteerRoleName),
            (SeedConstants.VolunteerTwoEmail, SeedConstants.DefaultVolunteerPassword, "Nikolay Petrov", SeedConstants.VolunteerRoleName),
            (SeedConstants.UserOneEmail, SeedConstants.DefaultUserPassword, "Elena Georgieva", SeedConstants.UserRoleName),
            (SeedConstants.UserTwoEmail, SeedConstants.DefaultUserPassword, "Georgi Dimitrov", SeedConstants.UserRoleName)
        };

        var seededUsers = new Dictionary<string, ApplicationUser>();

        foreach (var (email, password, fullName, role) in users)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    CreatedOn = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(user, password);
                EnsureSucceeded(createResult, $"creating user '{email}'");
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, role);
                EnsureSucceeded(addRoleResult, $"adding role '{role}' to user '{email}'");
            }

            seededUsers[email] = user;
        }

        return seededUsers;
    }

    private static async Task SeedDomainDataAsync(
        ApplicationDbContext dbContext,
        Dictionary<string, ApplicationUser> seededUsers,
        ILogger logger)
    {
        if (!await dbContext.ToolCategories.AnyAsync())
        {
            var categories = new List<ToolCategory>
            {
                new() { Name = "Electronics", Description = "Tools for diagnostics, soldering, and small device repair." },
                new() { Name = "Bicycle Repair", Description = "Bike stands, wrenches, tire tools, and alignment equipment." },
                new() { Name = "Sewing", Description = "Sewing and textile repair tools for community use." },
                new() { Name = "Woodworking", Description = "Hand and power tools for furniture and wood repairs." },
                new() { Name = "Home Maintenance", Description = "General-purpose tools for household fixes and improvements." }
            };

            await dbContext.ToolCategories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded tool categories.");
        }

        if (!await dbContext.Locations.AnyAsync())
        {
            var locations = new List<Location>
            {
                new()
                {
                    Name = "Sofia Community Workshop",
                    Address = "15 Vitosha Blvd",
                    City = "Sofia",
                    Description = "Main RepairCircle location with electronics benches and woodworking stations.",
                    Latitude = 42.695537m,
                    Longitude = 23.321901m
                },
                new()
                {
                    Name = "Green District Repair Hub",
                    Address = "28 Cherni Vrah Blvd",
                    City = "Sofia",
                    Description = "Neighborhood hub focused on bicycle and small appliance repairs.",
                    Latitude = 42.662994m,
                    Longitude = 23.318112m
                },
                new()
                {
                    Name = "Youth Maker Space",
                    Address = "7 Shipka Street",
                    City = "Plovdiv",
                    Description = "Youth-oriented repair and making space for learning and volunteering.",
                    Latitude = 42.147581m,
                    Longitude = 24.751614m
                }
            };

            await dbContext.Locations.AddRangeAsync(locations);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded locations.");
        }

        if (!await dbContext.Skills.AnyAsync())
        {
            var skills = new List<Skill>
            {
                new() { Name = "Soldering", Description = "Board-level soldering and connector replacement." },
                new() { Name = "Bicycle Tuning", Description = "Brake, gear, and wheel alignment service." },
                new() { Name = "Textile Repair", Description = "Zippers, seams, and fabric patching." },
                new() { Name = "Wood Repair", Description = "Furniture repair, sanding, and refinishing." },
                new() { Name = "Appliance Diagnostics", Description = "Basic diagnostics for common household appliances." }
            };

            await dbContext.Skills.AddRangeAsync(skills);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded skills.");
        }

        var volunteerUsers = new[]
        {
            seededUsers[SeedConstants.VolunteerOneEmail],
            seededUsers[SeedConstants.VolunteerTwoEmail]
        };

        if (!await dbContext.VolunteerProfiles.AnyAsync())
        {
            var volunteerProfiles = new List<VolunteerProfile>
            {
                new()
                {
                    UserId = volunteerUsers[0].Id,
                    Bio = "Electronics enthusiast with experience repairing small appliances and audio equipment.",
                    ExperienceLevel = ExperienceLevel.Advanced,
                    IsApproved = true
                },
                new()
                {
                    UserId = volunteerUsers[1].Id,
                    Bio = "Community bike mechanic and woodworking hobbyist helping at weekend repair events.",
                    ExperienceLevel = ExperienceLevel.Expert,
                    IsApproved = true
                }
            };

            await dbContext.VolunteerProfiles.AddRangeAsync(volunteerProfiles);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded volunteer profiles.");
        }

        await SeedVolunteerRelationsAsync(dbContext, logger);

        if (!await dbContext.Tools.AnyAsync())
        {
            var categories = await dbContext.ToolCategories.OrderBy(c => c.Id).ToListAsync();
            var locations = await dbContext.Locations.OrderBy(l => l.Id).ToListAsync();

            var tools = new List<Tool>
            {
                new()
                {
                    Name = "Digital Multimeter",
                    Description = "Portable diagnostic device for measuring voltage, resistance, and continuity.",
                    ImageUrl = "/images/tools/multimeter.jpg",
                    Condition = ToolCondition.Excellent,
                    IsAvailable = true,
                    Quantity = 4,
                    ToolCategoryId = categories.First(c => c.Name == "Electronics").Id,
                    LocationId = locations.First(l => l.Name == "Sofia Community Workshop").Id
                },
                new()
                {
                    Name = "Bike Repair Stand",
                    Description = "Adjustable stand for bicycle maintenance and repair sessions.",
                    ImageUrl = "/images/tools/bike-stand.jpg",
                    Condition = ToolCondition.Good,
                    IsAvailable = true,
                    Quantity = 3,
                    ToolCategoryId = categories.First(c => c.Name == "Bicycle Repair").Id,
                    LocationId = locations.First(l => l.Name == "Green District Repair Hub").Id
                },
                new()
                {
                    Name = "Sewing Machine",
                    Description = "Community sewing machine for clothing fixes and simple fabric repair tasks.",
                    ImageUrl = "/images/tools/sewing-machine.jpg",
                    Condition = ToolCondition.Good,
                    IsAvailable = true,
                    Quantity = 2,
                    ToolCategoryId = categories.First(c => c.Name == "Sewing").Id,
                    LocationId = locations.First(l => l.Name == "Youth Maker Space").Id
                },
                new()
                {
                    Name = "Orbital Sander",
                    Description = "Wood surface finishing tool used for furniture repair and restoration.",
                    ImageUrl = "/images/tools/orbital-sander.jpg",
                    Condition = ToolCondition.Fair,
                    IsAvailable = true,
                    Quantity = 2,
                    ToolCategoryId = categories.First(c => c.Name == "Woodworking").Id,
                    LocationId = locations.First(l => l.Name == "Sofia Community Workshop").Id
                },
                new()
                {
                    Name = "Cordless Drill Set",
                    Description = "General-purpose drill and bit set suitable for common home repairs.",
                    ImageUrl = "/images/tools/drill-set.jpg",
                    Condition = ToolCondition.Excellent,
                    IsAvailable = true,
                    Quantity = 5,
                    ToolCategoryId = categories.First(c => c.Name == "Home Maintenance").Id,
                    LocationId = locations.First(l => l.Name == "Green District Repair Hub").Id
                }
            };

            await dbContext.Tools.AddRangeAsync(tools);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded tools.");
        }

        if (!await dbContext.RepairSessions.AnyAsync())
        {
            var locations = await dbContext.Locations.OrderBy(l => l.Id).ToListAsync();

            var sessions = new List<RepairSession>
            {
                new()
                {
                    Title = "Weekend Electronics Repair Café",
                    Description = "Bring small appliances, speakers, radios, or power adapters for guided diagnostics and repair.",
                    StartDate = DateTime.UtcNow.Date.AddDays(7).AddHours(10),
                    EndDate = DateTime.UtcNow.Date.AddDays(7).AddHours(14),
                    MaxParticipants = 20,
                    AvailableSlots = 12,
                    LocationId = locations.First(l => l.Name == "Sofia Community Workshop").Id
                },
                new()
                {
                    Title = "Bike Tune-Up Community Day",
                    Description = "Volunteer-led session for brake tuning, puncture repair, and routine bicycle maintenance.",
                    StartDate = DateTime.UtcNow.Date.AddDays(10).AddHours(9),
                    EndDate = DateTime.UtcNow.Date.AddDays(10).AddHours(13),
                    MaxParticipants = 18,
                    AvailableSlots = 9,
                    LocationId = locations.First(l => l.Name == "Green District Repair Hub").Id
                }
            };

            await dbContext.RepairSessions.AddRangeAsync(sessions);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded repair sessions.");
        }

        await SeedRepairSessionVolunteersAsync(dbContext, logger);

        if (!await dbContext.RepairRequests.AnyAsync())
        {
            var locations = await dbContext.Locations.ToListAsync();
            var sessions = await dbContext.RepairSessions.OrderBy(rs => rs.Id).ToListAsync();
            var volunteerProfiles = await dbContext.VolunteerProfiles.OrderBy(vp => vp.Id).ToListAsync();

            var repairRequests = new List<RepairRequest>
            {
                new()
                {
                    Title = "Speaker no longer powers on",
                    Description = "Portable Bluetooth speaker stopped powering on after a charging issue. Looking for diagnosis and possible battery replacement.",
                    ItemType = "Small Electronics",
                    ImageUrl = "/images/requests/speaker.jpg",
                    Status = RepairRequestStatus.InProgress,
                    SubmittedByUserId = seededUsers[SeedConstants.UserOneEmail].Id,
                    AssignedVolunteerProfileId = volunteerProfiles[0].Id,
                    LocationId = locations.First(l => l.Name == "Sofia Community Workshop").Id,
                    RepairSessionId = sessions[0].Id,
                    RequestedDate = DateTime.UtcNow.AddDays(-4)
                },
                new()
                {
                    Title = "City bicycle rear brake adjustment",
                    Description = "Rear brake has become soft and the wheel rubs slightly after transport. Needs checking before daily commuting.",
                    ItemType = "Bicycle",
                    ImageUrl = "/images/requests/bike.jpg",
                    Status = RepairRequestStatus.Approved,
                    SubmittedByUserId = seededUsers[SeedConstants.UserTwoEmail].Id,
                    AssignedVolunteerProfileId = volunteerProfiles[1].Id,
                    LocationId = locations.First(l => l.Name == "Green District Repair Hub").Id,
                    RepairSessionId = sessions[1].Id,
                    RequestedDate = DateTime.UtcNow.AddDays(-2)
                },
                new()
                {
                    Title = "Jacket zipper replacement",
                    Description = "Winter jacket zipper teeth are broken near the middle and the zipper no longer closes smoothly.",
                    ItemType = "Clothing",
                    ImageUrl = "/images/requests/jacket.jpg",
                    Status = RepairRequestStatus.Submitted,
                    SubmittedByUserId = seededUsers[SeedConstants.UserOneEmail].Id,
                    LocationId = locations.First(l => l.Name == "Youth Maker Space").Id,
                    RequestedDate = DateTime.UtcNow.AddDays(-1)
                }
            };

            await dbContext.RepairRequests.AddRangeAsync(repairRequests);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded repair requests.");
        }

        if (!await dbContext.BorrowRecords.AnyAsync())
        {
            var tools = await dbContext.Tools.ToListAsync();

            var borrowRecords = new List<BorrowRecord>
            {
                new()
                {
                    ToolId = tools.First(t => t.Name == "Cordless Drill Set").Id,
                    UserId = seededUsers[SeedConstants.UserOneEmail].Id,
                    BorrowDate = DateTime.UtcNow.AddDays(-3),
                    DueDate = DateTime.UtcNow.AddDays(4),
                    Status = BorrowStatus.Borrowed,
                    BorrowReference = "BRW-2026-0001"
                },
                new()
                {
                    ToolId = tools.First(t => t.Name == "Digital Multimeter").Id,
                    UserId = seededUsers[SeedConstants.UserTwoEmail].Id,
                    BorrowDate = DateTime.UtcNow.AddDays(-8),
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    ReturnedDate = DateTime.UtcNow.AddDays(-2),
                    Status = BorrowStatus.Returned,
                    BorrowReference = "BRW-2026-0002"
                }
            };

            await dbContext.BorrowRecords.AddRangeAsync(borrowRecords);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded borrow records.");
        }

        if (!await dbContext.Feedbacks.AnyAsync())
        {
            var completedRequest = await dbContext.RepairRequests
                .OrderBy(r => r.Id)
                .FirstOrDefaultAsync(r => r.Title.Contains("Speaker", StringComparison.OrdinalIgnoreCase));

            if (completedRequest is not null)
            {
                completedRequest.Status = RepairRequestStatus.Repaired;

                var feedback = new Feedback
                {
                    UserId = seededUsers[SeedConstants.UserOneEmail].Id,
                    RepairRequestId = completedRequest.Id,
                    Rating = 5,
                    Comment = "Excellent help. The issue was diagnosed clearly and the speaker was repaired during the workshop."
                };

                await dbContext.Feedbacks.AddAsync(feedback);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded feedback.");
            }
        }

        if (!await dbContext.Favorites.AnyAsync())
        {
            var drill = await dbContext.Tools.FirstAsync(t => t.Name == "Cordless Drill Set");
            var bikeStand = await dbContext.Tools.FirstAsync(t => t.Name == "Bike Repair Stand");

            var favorites = new List<Favorite>
            {
                new() { UserId = seededUsers[SeedConstants.UserOneEmail].Id, ToolId = drill.Id },
                new() { UserId = seededUsers[SeedConstants.UserTwoEmail].Id, ToolId = bikeStand.Id }
            };

            await dbContext.Favorites.AddRangeAsync(favorites);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded favorites.");
        }

        if (!await dbContext.Announcements.AnyAsync())
        {
            var announcements = new List<Announcement>
            {
                new()
                {
                    Title = "RepairCircle launches spring repair weekends",
                    Content = "New weekend repair cafés are now open for registrations in Sofia and Plovdiv. Volunteers can sign up from their profile pages.",
                    IsImportant = true,
                    IsPublished = true
                },
                new()
                {
                    Title = "Tool lending reminder",
                    Content = "Please return borrowed tools on time so other community members can reserve them. Late returns reduce tool availability for upcoming workshops.",
                    IsImportant = false,
                    IsPublished = true
                }
            };

            await dbContext.Announcements.AddRangeAsync(announcements);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded announcements.");
        }
    }

    private static async Task SeedVolunteerRelationsAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var profiles = await dbContext.VolunteerProfiles
            .Include(vp => vp.Skills)
            .OrderBy(vp => vp.Id)
            .ToListAsync();

        var skills = await dbContext.Skills.OrderBy(s => s.Id).ToListAsync();

        if (profiles.Count < 2 || skills.Count < 5)
        {
            return;
        }

        var maria = profiles[0];
        var nikolay = profiles[1];

        if (!maria.Skills.Any())
        {
            maria.Skills.Add(skills.First(s => s.Name == "Soldering"));
            maria.Skills.Add(skills.First(s => s.Name == "Appliance Diagnostics"));
        }

        if (!nikolay.Skills.Any())
        {
            nikolay.Skills.Add(skills.First(s => s.Name == "Bicycle Tuning"));
            nikolay.Skills.Add(skills.First(s => s.Name == "Wood Repair"));
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded volunteer skill relations.");
    }

    private static async Task SeedRepairSessionVolunteersAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var sessions = await dbContext.RepairSessions
            .Include(rs => rs.Volunteers)
            .OrderBy(rs => rs.Id)
            .ToListAsync();

        var profiles = await dbContext.VolunteerProfiles.OrderBy(vp => vp.Id).ToListAsync();

        if (!sessions.Any() || profiles.Count < 2)
        {
            return;
        }

        var electronicsSession = sessions.FirstOrDefault(rs => rs.Title.Contains("Electronics", StringComparison.OrdinalIgnoreCase));
        var bikeSession = sessions.FirstOrDefault(rs => rs.Title.Contains("Bike", StringComparison.OrdinalIgnoreCase));

        if (electronicsSession is not null && !electronicsSession.Volunteers.Any(v => v.Id == profiles[0].Id))
        {
            electronicsSession.Volunteers.Add(profiles[0]);
        }

        if (bikeSession is not null && !bikeSession.Volunteers.Any(v => v.Id == profiles[1].Id))
        {
            bikeSession.Volunteers.Add(profiles[1]);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded repair session volunteer relations.");
    }

    private static void EnsureSucceeded(IdentityResult result, string action)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Error {action}: {errors}");
    }
}
