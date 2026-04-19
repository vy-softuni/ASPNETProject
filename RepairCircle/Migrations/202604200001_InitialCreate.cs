using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RepairCircle.Data;

#nullable disable

namespace RepairCircle.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("202604200001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Announcements]', N'U') IS NULL
BEGIN
    CREATE TABLE [Announcements] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(150) NOT NULL,
        [Content] nvarchar(2000) NOT NULL,
        [IsImportant] bit NOT NULL,
        [IsPublished] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_Announcements] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[AspNetRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[AspNetUsers]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[Locations]', N'U') IS NULL
BEGIN
    CREATE TABLE [Locations] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Address] nvarchar(200) NOT NULL,
        [City] nvarchar(80) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Latitude] decimal(9,6) NULL,
        [Longitude] decimal(9,6) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_Locations] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[Skills]', N'U') IS NULL
BEGIN
    CREATE TABLE [Skills] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        [Description] nvarchar(300) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_Skills] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[ToolCategories]', N'U') IS NULL
BEGIN
    CREATE TABLE [ToolCategories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(60) NOT NULL,
        [Description] nvarchar(250) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_ToolCategories] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[AspNetRoleClaims]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[AspNetUserClaims]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[AspNetUserLogins]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[AspNetUserRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[AspNetUserTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[VolunteerProfiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [VolunteerProfiles] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Bio] nvarchar(1000) NULL,
        [ExperienceLevel] int NOT NULL,
        [IsApproved] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_VolunteerProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VolunteerProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF OBJECT_ID(N'[RepairSessions]', N'U') IS NULL
BEGIN
    CREATE TABLE [RepairSessions] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(120) NOT NULL,
        [Description] nvarchar(1500) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [MaxParticipants] int NOT NULL,
        [AvailableSlots] int NOT NULL,
        [LocationId] int NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_RepairSessions] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_RepairSessions_AvailableSlots_Valid] CHECK ([AvailableSlots] >= 0 AND [AvailableSlots] <= [MaxParticipants]),
        CONSTRAINT [CK_RepairSessions_EndDate_After_StartDate] CHECK ([EndDate] >= [StartDate]),
        CONSTRAINT [FK_RepairSessions_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION
    );
END;

IF OBJECT_ID(N'[Tools]', N'U') IS NULL
BEGIN
    CREATE TABLE [Tools] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(1500) NOT NULL,
        [ImageUrl] nvarchar(255) NULL,
        [Condition] int NOT NULL,
        [IsAvailable] bit NOT NULL,
        [Quantity] int NOT NULL,
        [ToolCategoryId] int NOT NULL,
        [LocationId] int NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_Tools] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Tools_Quantity_NonNegative] CHECK ([Quantity] >= 0),
        CONSTRAINT [FK_Tools_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Tools_ToolCategories_ToolCategoryId] FOREIGN KEY ([ToolCategoryId]) REFERENCES [ToolCategories] ([Id]) ON DELETE NO ACTION
    );
END;

IF OBJECT_ID(N'[VolunteerProfileSkills]', N'U') IS NULL
BEGIN
    CREATE TABLE [VolunteerProfileSkills] (
        [VolunteerProfileId] int NOT NULL,
        [SkillId] int NOT NULL,
        CONSTRAINT [PK_VolunteerProfileSkills] PRIMARY KEY ([VolunteerProfileId], [SkillId]),
        CONSTRAINT [FK_VolunteerProfileSkill_Skills_SkillId] FOREIGN KEY ([SkillId]) REFERENCES [Skills] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_VolunteerProfileSkill_VolunteerProfiles_VolunteerProfileId] FOREIGN KEY ([VolunteerProfileId]) REFERENCES [VolunteerProfiles] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[RepairRequests]', N'U') IS NULL
BEGIN
    CREATE TABLE [RepairRequests] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(120) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [ItemType] nvarchar(80) NOT NULL,
        [ImageUrl] nvarchar(255) NULL,
        [RequestReference] nvarchar(50) NOT NULL,
        [Status] int NOT NULL,
        [SubmittedByUserId] nvarchar(450) NOT NULL,
        [AssignedVolunteerProfileId] int NULL,
        [LocationId] int NOT NULL,
        [RepairSessionId] int NULL,
        [RequestedDate] datetime2 NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_RepairRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RepairRequests_AspNetUsers_SubmittedByUserId] FOREIGN KEY ([SubmittedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RepairRequests_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RepairRequests_RepairSessions_RepairSessionId] FOREIGN KEY ([RepairSessionId]) REFERENCES [RepairSessions] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_RepairRequests_VolunteerProfiles_AssignedVolunteerProfileId] FOREIGN KEY ([AssignedVolunteerProfileId]) REFERENCES [VolunteerProfiles] ([Id]) ON DELETE SET NULL
    );
END;

IF OBJECT_ID(N'[RepairSessionVolunteers]', N'U') IS NULL
BEGIN
    CREATE TABLE [RepairSessionVolunteers] (
        [RepairSessionId] int NOT NULL,
        [VolunteerProfileId] int NOT NULL,
        CONSTRAINT [PK_RepairSessionVolunteers] PRIMARY KEY ([RepairSessionId], [VolunteerProfileId]),
        CONSTRAINT [FK_RepairSessionVolunteer_RepairSessions_RepairSessionId] FOREIGN KEY ([RepairSessionId]) REFERENCES [RepairSessions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RepairSessionVolunteer_VolunteerProfiles_VolunteerProfileId] FOREIGN KEY ([VolunteerProfileId]) REFERENCES [VolunteerProfiles] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[BorrowRecords]', N'U') IS NULL
BEGIN
    CREATE TABLE [BorrowRecords] (
        [Id] int NOT NULL IDENTITY,
        [ToolId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [BorrowDate] datetime2 NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [ReturnedDate] datetime2 NULL,
        [Status] int NOT NULL,
        [BorrowReference] nvarchar(50) NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_BorrowRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_BorrowRecords_DueDate_After_BorrowDate] CHECK ([DueDate] >= [BorrowDate]),
        CONSTRAINT [FK_BorrowRecords_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BorrowRecords_Tools_ToolId] FOREIGN KEY ([ToolId]) REFERENCES [Tools] ([Id]) ON DELETE NO ACTION
    );
END;

IF OBJECT_ID(N'[Favorites]', N'U') IS NULL
BEGIN
    CREATE TABLE [Favorites] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ToolId] int NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_Favorites] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Favorites_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Favorites_Tools_ToolId] FOREIGN KEY ([ToolId]) REFERENCES [Tools] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[Feedbacks]', N'U') IS NULL
BEGIN
    CREATE TABLE [Feedbacks] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [RepairRequestId] int NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(1000) NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Feedbacks_RepairRequests_RepairRequestId] FOREIGN KEY ([RepairRequestId]) REFERENCES [RepairRequests] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetRoleClaims_RoleId' AND object_id = OBJECT_ID(N'[AspNetRoleClaims]'))
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'RoleNameIndex' AND object_id = OBJECT_ID(N'[AspNetRoles]'))
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserClaims_UserId' AND object_id = OBJECT_ID(N'[AspNetUserClaims]'))
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserLogins_UserId' AND object_id = OBJECT_ID(N'[AspNetUserLogins]'))
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserRoles_RoleId' AND object_id = OBJECT_ID(N'[AspNetUserRoles]'))
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'EmailIndex' AND object_id = OBJECT_ID(N'[AspNetUsers]'))
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UserNameIndex' AND object_id = OBJECT_ID(N'[AspNetUsers]'))
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_BorrowRecords_BorrowReference' AND object_id = OBJECT_ID(N'[BorrowRecords]'))
    CREATE UNIQUE INDEX [IX_BorrowRecords_BorrowReference] ON [BorrowRecords] ([BorrowReference]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_BorrowRecords_ToolId' AND object_id = OBJECT_ID(N'[BorrowRecords]'))
    CREATE INDEX [IX_BorrowRecords_ToolId] ON [BorrowRecords] ([ToolId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_BorrowRecords_UserId' AND object_id = OBJECT_ID(N'[BorrowRecords]'))
    CREATE INDEX [IX_BorrowRecords_UserId] ON [BorrowRecords] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Favorites_ToolId' AND object_id = OBJECT_ID(N'[Favorites]'))
    CREATE INDEX [IX_Favorites_ToolId] ON [Favorites] ([ToolId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Favorites_UserId_ToolId' AND object_id = OBJECT_ID(N'[Favorites]'))
    CREATE UNIQUE INDEX [IX_Favorites_UserId_ToolId] ON [Favorites] ([UserId], [ToolId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Feedbacks_RepairRequestId' AND object_id = OBJECT_ID(N'[Feedbacks]'))
    CREATE INDEX [IX_Feedbacks_RepairRequestId] ON [Feedbacks] ([RepairRequestId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Feedbacks_UserId_RepairRequestId' AND object_id = OBJECT_ID(N'[Feedbacks]'))
    CREATE UNIQUE INDEX [IX_Feedbacks_UserId_RepairRequestId] ON [Feedbacks] ([UserId], [RepairRequestId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RepairRequests_AssignedVolunteerProfileId' AND object_id = OBJECT_ID(N'[RepairRequests]'))
    CREATE INDEX [IX_RepairRequests_AssignedVolunteerProfileId] ON [RepairRequests] ([AssignedVolunteerProfileId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RepairRequests_LocationId' AND object_id = OBJECT_ID(N'[RepairRequests]'))
    CREATE INDEX [IX_RepairRequests_LocationId] ON [RepairRequests] ([LocationId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RepairRequests_RepairSessionId' AND object_id = OBJECT_ID(N'[RepairRequests]'))
    CREATE INDEX [IX_RepairRequests_RepairSessionId] ON [RepairRequests] ([RepairSessionId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RepairRequests_RequestReference' AND object_id = OBJECT_ID(N'[RepairRequests]'))
    CREATE UNIQUE INDEX [IX_RepairRequests_RequestReference] ON [RepairRequests] ([RequestReference]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RepairRequests_SubmittedByUserId' AND object_id = OBJECT_ID(N'[RepairRequests]'))
    CREATE INDEX [IX_RepairRequests_SubmittedByUserId] ON [RepairRequests] ([SubmittedByUserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RepairSessions_LocationId' AND object_id = OBJECT_ID(N'[RepairSessions]'))
    CREATE INDEX [IX_RepairSessions_LocationId] ON [RepairSessions] ([LocationId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RepairSessionVolunteers_VolunteerProfileId' AND object_id = OBJECT_ID(N'[RepairSessionVolunteers]'))
    CREATE INDEX [IX_RepairSessionVolunteers_VolunteerProfileId] ON [RepairSessionVolunteers] ([VolunteerProfileId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Skills_Name' AND object_id = OBJECT_ID(N'[Skills]'))
    CREATE UNIQUE INDEX [IX_Skills_Name] ON [Skills] ([Name]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ToolCategories_Name' AND object_id = OBJECT_ID(N'[ToolCategories]'))
    CREATE UNIQUE INDEX [IX_ToolCategories_Name] ON [ToolCategories] ([Name]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Tools_LocationId' AND object_id = OBJECT_ID(N'[Tools]'))
    CREATE INDEX [IX_Tools_LocationId] ON [Tools] ([LocationId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Tools_ToolCategoryId' AND object_id = OBJECT_ID(N'[Tools]'))
    CREATE INDEX [IX_Tools_ToolCategoryId] ON [Tools] ([ToolCategoryId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_VolunteerProfiles_UserId' AND object_id = OBJECT_ID(N'[VolunteerProfiles]'))
    CREATE UNIQUE INDEX [IX_VolunteerProfiles_UserId] ON [VolunteerProfiles] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_VolunteerProfileSkills_SkillId' AND object_id = OBJECT_ID(N'[VolunteerProfileSkills]'))
    CREATE INDEX [IX_VolunteerProfileSkills_SkillId] ON [VolunteerProfileSkills] ([SkillId]);
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Feedbacks]', N'U') IS NOT NULL DROP TABLE [Feedbacks];
IF OBJECT_ID(N'[Favorites]', N'U') IS NOT NULL DROP TABLE [Favorites];
IF OBJECT_ID(N'[BorrowRecords]', N'U') IS NOT NULL DROP TABLE [BorrowRecords];
IF OBJECT_ID(N'[RepairSessionVolunteers]', N'U') IS NOT NULL DROP TABLE [RepairSessionVolunteers];
IF OBJECT_ID(N'[RepairRequests]', N'U') IS NOT NULL DROP TABLE [RepairRequests];
IF OBJECT_ID(N'[VolunteerProfileSkills]', N'U') IS NOT NULL DROP TABLE [VolunteerProfileSkills];
IF OBJECT_ID(N'[Tools]', N'U') IS NOT NULL DROP TABLE [Tools];
IF OBJECT_ID(N'[RepairSessions]', N'U') IS NOT NULL DROP TABLE [RepairSessions];
IF OBJECT_ID(N'[VolunteerProfiles]', N'U') IS NOT NULL DROP TABLE [VolunteerProfiles];
IF OBJECT_ID(N'[AspNetUserTokens]', N'U') IS NOT NULL DROP TABLE [AspNetUserTokens];
IF OBJECT_ID(N'[AspNetUserRoles]', N'U') IS NOT NULL DROP TABLE [AspNetUserRoles];
IF OBJECT_ID(N'[AspNetUserLogins]', N'U') IS NOT NULL DROP TABLE [AspNetUserLogins];
IF OBJECT_ID(N'[AspNetUserClaims]', N'U') IS NOT NULL DROP TABLE [AspNetUserClaims];
IF OBJECT_ID(N'[AspNetRoleClaims]', N'U') IS NOT NULL DROP TABLE [AspNetRoleClaims];
IF OBJECT_ID(N'[ToolCategories]', N'U') IS NOT NULL DROP TABLE [ToolCategories];
IF OBJECT_ID(N'[Skills]', N'U') IS NOT NULL DROP TABLE [Skills];
IF OBJECT_ID(N'[Locations]', N'U') IS NOT NULL DROP TABLE [Locations];
IF OBJECT_ID(N'[AspNetUsers]', N'U') IS NOT NULL DROP TABLE [AspNetUsers];
IF OBJECT_ID(N'[AspNetRoles]', N'U') IS NOT NULL DROP TABLE [AspNetRoles];
IF OBJECT_ID(N'[Announcements]', N'U') IS NOT NULL DROP TABLE [Announcements];
");
    }
}
