# RepairCircle — Requirements Compliance and Examiner Checklist

> **Scope of this file**
>
> This document maps the assignment requirements to the **current project structure** and points to the **exact files and line ranges** an examiner can inspect.
>
> **Reference project used for this checklist:** `RepairCircle-BUILTIN-PICS-UI-fixed-v2`.
>
> If you check a later cosmetic variant of the same project, some line numbers may shift slightly, but the structure and feature placement should remain the same.

---

## 1. Quick count summary

### 1.1 Controllers — **15 total** (requirement: at least 5)

**Public controllers**
- `Controllers/HomeController.cs`
- `Controllers/ToolsController.cs`
- `Controllers/RepairRequestsController.cs`
- `Controllers/RepairSessionsController.cs`
- `Controllers/BorrowRecordsController.cs`
- `Controllers/FavoritesController.cs`
- `Controllers/FeedbackController.cs`
- `Controllers/VolunteersController.cs`

**Admin controllers**
- `Areas/Admin/Controllers/DashboardController.cs`
- `Areas/Admin/Controllers/ToolsController.cs`
- `Areas/Admin/Controllers/RepairRequestsController.cs`
- `Areas/Admin/Controllers/RepairSessionsController.cs`
- `Areas/Admin/Controllers/LocationsController.cs`
- `Areas/Admin/Controllers/AnnouncementsController.cs`
- `Areas/Admin/Controllers/UsersController.cs`

### 1.2 Entity/domain models — **13 total** (requirement: at least 6)

- `Data/Models/ApplicationUser.cs`
- `Data/Models/BaseEntity.cs`
- `Data/Models/Announcement.cs`
- `Data/Models/BorrowRecord.cs`
- `Data/Models/Favorite.cs`
- `Data/Models/Feedback.cs`
- `Data/Models/Location.cs`
- `Data/Models/RepairRequest.cs`
- `Data/Models/RepairSession.cs`
- `Data/Models/Skill.cs`
- `Data/Models/Tool.cs`
- `Data/Models/ToolCategory.cs`
- `Data/Models/VolunteerProfile.cs`

### 1.3 Concrete views/pages — **39 total** (requirement: at least 15)

**Public / shared pages**
- `Views/Home/Index.cshtml`
- `Views/Home/About.cshtml`
- `Views/Home/Contact.cshtml`
- `Views/Home/AccessDenied.cshtml`
- `Views/Home/NotFound.cshtml`
- `Views/Home/ServerError.cshtml`
- `Views/Tools/Index.cshtml`
- `Views/Tools/Details.cshtml`
- `Views/RepairRequests/Index.cshtml`
- `Views/RepairRequests/Create.cshtml`
- `Views/RepairRequests/Details.cshtml`
- `Views/RepairSessions/Index.cshtml`
- `Views/RepairSessions/Details.cshtml`
- `Views/BorrowRecords/Create.cshtml`
- `Views/BorrowRecords/MyRecords.cshtml`
- `Views/BorrowRecords/Details.cshtml`
- `Views/Favorites/Index.cshtml`
- `Views/Feedback/Create.cshtml`
- `Views/Feedback/Edit.cshtml`
- `Views/Volunteers/Index.cshtml`
- `Views/Volunteers/BecomeOne.cshtml`
- `Views/Shared/Error.cshtml`

**Admin pages**
- `Areas/Admin/Views/Dashboard/Index.cshtml`
- `Areas/Admin/Views/Tools/Index.cshtml`
- `Areas/Admin/Views/Tools/Create.cshtml`
- `Areas/Admin/Views/Tools/Edit.cshtml`
- `Areas/Admin/Views/RepairRequests/Index.cshtml`
- `Areas/Admin/Views/RepairRequests/Details.cshtml`
- `Areas/Admin/Views/RepairSessions/Index.cshtml`
- `Areas/Admin/Views/RepairSessions/Create.cshtml`
- `Areas/Admin/Views/RepairSessions/Edit.cshtml`
- `Areas/Admin/Views/Locations/Index.cshtml`
- `Areas/Admin/Views/Locations/Create.cshtml`
- `Areas/Admin/Views/Locations/Edit.cshtml`
- `Areas/Admin/Views/Announcements/Index.cshtml`
- `Areas/Admin/Views/Announcements/Create.cshtml`
- `Areas/Admin/Views/Announcements/Edit.cshtml`
- `Areas/Admin/Views/Users/Index.cshtml`

**Identity pages**
- `Areas/Identity/Pages/Account/Login.cshtml`
- `Areas/Identity/Pages/Account/Register.cshtml`

---

## 2. General requirements mapping

### 2.1 ASP.NET Core Framework — **fulfilled**
- Target framework is `.NET 8.0` in `RepairCircle.csproj:1-6`.
- ASP.NET Core MVC project SDK is used in `RepairCircle.csproj:1`.

### 2.2 Visual Studio / Razor template engine / sections / partial views — **fulfilled**
- Razor view engine is used throughout `Views/` and `Areas/*/Views/`.
- Layout sections are rendered in `Views/Shared/_Layout.cshtml:7-8, 65` and `Areas/Admin/Views/Shared/_AdminLayout.cshtml:10-11, 62`.
- Example pages using sections:
  - `Areas/Identity/Pages/Account/Login.cshtml:68-70`
  - `Areas/Identity/Pages/Account/Register.cshtml:73-75`
  - `Views/RepairRequests/Create.cshtml:88-91`
  - `Views/Tools/Details.cshtml:115-127`
- Partial views exist in `Views/Shared/` and `Areas/Admin/Views/Shared/`, for example:
  - `Views/Shared/_ToolCardPartial.cshtml`
  - `Views/Shared/_RepairRequestCardPartial.cshtml`
  - `Views/Shared/_FeedbackListPartial.cshtml`
  - `Views/Shared/_LocationMapPartial.cshtml`
  - `Areas/Admin/Views/Shared/_ToolFormPartial.cshtml`
  - `Areas/Admin/Views/Shared/_LocationFormPartial.cshtml`
  - `Areas/Admin/Views/Shared/_RepairSessionFormPartial.cshtml`
  - `Areas/Admin/Views/Shared/_AnnouncementFormPartial.cshtml`

### 2.3 Microsoft SQL Server database service — **fulfilled**
- SQL Server / LocalDB connection string in `appsettings.json:1-11`.
- SQL Server provider package in `RepairCircle.csproj:16-20`.
- EF Core SQL Server configuration in `Program.cs:11-15`.

### 2.4 Entity Framework Core access — **fulfilled**
- EF Core packages in `RepairCircle.csproj:12-20`.
- `ApplicationDbContext` defined in `Data/ApplicationDbContext.cs:7-25`.
- Model configuration and relationships in `Data/ApplicationDbContext.cs:26-214`.
- Migration support in:
  - `Migrations/202604200001_InitialCreate.cs`
  - `Migrations/ApplicationDbContextModelSnapshot.cs`
  - `Data/ApplicationDbContextFactory.cs`

### 2.5 MVC Areas — **fulfilled**
- Admin area routing is mapped in `Program.cs:64-66`.
- Admin area structure exists under `Areas/Admin/Controllers/` and `Areas/Admin/Views/`.
- Dedicated admin layout and view start:
  - `Areas/Admin/Views/Shared/_AdminLayout.cshtml:1-64`
  - `Areas/Admin/Views/_ViewStart.cshtml`

### 2.6 Responsive design / own design — **fulfilled**
- The project uses a custom responsive design rather than Bootstrap.
- Responsive viewport is set in:
  - `Views/Shared/_Layout.cshtml:3-8`
  - `Areas/Admin/Views/Shared/_AdminLayout.cshtml:6-11`
- Responsive CSS and media queries exist in `wwwroot/css/site.css`, for example:
  - general layout and cards `site.css:39-176`
  - responsive media queries `site.css:677-718`, `site.css:853-897`, `site.css:1188-1191`, `site.css:1429-1434`, `site.css:1530-1532`, `site.css:1607`
  - styled login/register pages `site.css:1570-1616`

### 2.7 Standard ASP.NET Identity system for users and roles — **fulfilled**
- Identity setup in `Program.cs:19-35`.
- Custom user class extends IdentityUser in `Data/Models/ApplicationUser.cs:5-13`.
- Roles are enabled with `.AddRoles<IdentityRole>()` in `Program.cs:19-30`.
- Custom login/register pages exist in:
  - `Areas/Identity/Pages/Account/Login.cshtml` + `.cshtml.cs`
  - `Areas/Identity/Pages/Account/Register.cshtml` + `.cshtml.cs`
- Roles are seeded in `Data/Seed/ApplicationDbInitializer.cs:36-53`.
- Users are seeded and assigned to roles in `Data/Seed/ApplicationDbInitializer.cs:55-97`.

### 2.8 Required roles: User and Administrator — **fulfilled**
- Roles seeded in `Data/Seed/ApplicationDbInitializer.cs:38-42`.
- Specific assignment of admin and user accounts in `Data/Seed/ApplicationDbInitializer.cs:57-64, 87-93`.
- Admin-only controllers use role authorization, e.g.:
  - `Areas/Admin/Controllers/DashboardController.cs:8`
  - `Areas/Admin/Controllers/ToolsController.cs:14`
  - `Areas/Admin/Controllers/RepairRequestsController.cs:13`
  - `Areas/Admin/Controllers/RepairSessionsController.cs:12`
  - `Areas/Admin/Controllers/LocationsController.cs:11`
  - `Areas/Admin/Controllers/AnnouncementsController.cs:11`
  - `Areas/Admin/Controllers/UsersController.cs:12`

### 2.9 AJAX request(s) to asynchronously load/display data — **fulfilled**
- AJAX favorites toggle:
  - `wwwroot/js/ajax-favorites.js:1-60`
  - `Controllers/FavoritesController.cs` POST actions
  - `Views/Tools/Details.cshtml:76-90, 124-126`
- AJAX feedback loading and submission:
  - `wwwroot/js/repair-request-feedback.js:1-150`
  - `Controllers/FeedbackController.cs:108-139` (partial and AJAX handling)
  - `Views/RepairRequests/Details.cshtml` (feedback area and script section)

### 2.10 Error handling and data validation — **fulfilled**

#### Server-side validation
Examples:
- `Data/Models/Tool.cs:9-31`
- `Data/Models/RepairRequest.cs:9-31`
- `Data/Models/RepairSession.cs:8-59`
- `Data/Models/Location.cs:7-49`
- `Data/Models/BorrowRecord.cs:18-52`
- `Areas/Identity/Pages/Account/Register.cshtml.cs:35-57`
- `Areas/Identity/Pages/Account/Login.cshtml.cs:28-39`

#### Database-level validation / constraints
- unique indexes, decimal precision, relationship rules in `Data/ApplicationDbContext.cs:30-64, 66-214`
- check constraints in:
  - `Data/ApplicationDbContext.cs:66-83`
- uniqueness rules:
  - `Data/ApplicationDbContext.cs:30-56`

#### Client-side validation
- `_ValidationScriptsPartial` is used in create/edit/login/register pages, e.g.:
  - `Views/RepairRequests/Create.cshtml:88-90`
  - `Areas/Identity/Pages/Account/Login.cshtml:68-70`
  - `Areas/Identity/Pages/Account/Register.cshtml:73-75`

### 2.11 Custom 404 and 500 pages — **fulfilled**
- Exception handler and status-code re-execution in `Program.cs:44-55`.
- Home controller error endpoints in `Controllers/HomeController.cs:30-76`.
- Custom views:
  - `Views/Home/NotFound.cshtml`
  - `Views/Home/ServerError.cshtml`
  - `Views/Home/AccessDenied.cshtml`
  - `Views/Shared/Error.cshtml`

### 2.12 Correct handling of special HTML characters / tags — **fulfilled by Razor default encoding**
- User content is rendered through normal Razor expressions, which HTML-encode by default.
- Example pages rendering user-entered fields without `Html.Raw`:
  - `Views/Tools/Details.cshtml:32-60`
  - `Views/RepairRequests/Details.cshtml` (title, description, item type, submitted-by fields)
  - `Views/Volunteers/Index.cshtml:82-85`
- The only `Html.Raw` uses are for serialized JSON payloads / chart data, not for free-form user HTML, e.g.:
  - `Areas/Admin/Views/Dashboard/Index.cshtml:146-151`
  - `Views/Tools/Details.cshtml:122`

### 2.13 Dependency Injection — **fulfilled**
- DbContext registered in `Program.cs:14-15`.
- Identity registered in `Program.cs:19-35`.
- Application services registered in `Program.cs:37-40` and `Services/ServiceCollectionExtensions.cs:7-24`.
- Constructor injection used in controllers and services, e.g.:
  - `Controllers/HomeController.cs:11-16`
  - `Services/Implementations/ToolService.cs:12-17`
  - `Services/Implementations/RepairRequestService.cs:14-21`

### 2.14 Pagination functionality — **fulfilled**
- Reusable pagination model:
  - `ViewModels/Common/PaginationViewModel.cs`
- Tool pagination logic in `Services/Implementations/ToolService.cs:19-27, 72-80, 127-132`
- Repair-request pagination logic in `Services/Implementations/RepairRequestService.cs:23-30, 200-257`
- Example paginated views:
  - `Views/Tools/Index.cshtml:14-23, 119-157`
  - `Views/RepairRequests/Index.cshtml:13-22` and later pagination block
  - `Views/BorrowRecords/MyRecords.cshtml`
  - `Views/Volunteers/Index.cshtml`
  - `Views/RepairSessions/Index.cshtml:12-20, 93-131`
  - admin list views under `Areas/Admin/Views/*/Index.cshtml`

### 2.15 Search and filtering functionality — **fulfilled**
- Tool filtering/search in `Services/Implementations/ToolService.cs:19-26, 41-70` and `Views/Tools/Index.cshtml:31-80`.
- Repair-request filtering/search in `Services/Implementations/RepairRequestService.cs:23-30, 222-247` and `Views/RepairRequests/Index.cshtml:38-74`.
- Session filtering/search in `Views/RepairSessions/Index.cshtml:29-50` and service logic in `Services/Implementations/RepairSessionService.cs`.
- Volunteer filtering/search in `Views/Volunteers/Index.cshtml` and `Services/Implementations/VolunteerService.cs`.
- Borrowing/favorites/admin search is also present in their respective index services/views.

### 2.16 Seeding data — **fulfilled**
- Database initialization and seeding entry point: `Data/Seed/ApplicationDbInitializer.cs:11-34`.
- Roles: `Data/Seed/ApplicationDbInitializer.cs:36-53`.
- Users: `Data/Seed/ApplicationDbInitializer.cs:55-97`.
- Domain data seeding:
  - categories `Data/Seed/ApplicationDbInitializer.cs:104-118`
  - locations `Data/Seed/ApplicationDbInitializer.cs:120-156`
  - skills `Data/Seed/ApplicationDbInitializer.cs:158-172`
  - volunteer profiles `Data/Seed/ApplicationDbInitializer.cs:180-203`
  - tools `Data/Seed/ApplicationDbInitializer.cs:207-299`
  - repair sessions, repair requests, borrow records, feedback, favorites, announcements later in the same file
- Seed constants in `Data/Seed/SeedConstants.cs`.

### 2.17 Security protections (SQL injection, XSS, CSRF, parameter tampering, anti-forgery) — **fulfilled**

#### SQL injection protection
- EF Core LINQ is used throughout services, not raw string-concatenated SQL, e.g.:
  - `Services/Implementations/ToolService.cs:33-114`
  - `Services/Implementations/RepairRequestService.cs:30-31, 121-197, 213-257`
  - `Services/Implementations/AdminDashboardService.cs:27-98`

#### XSS mitigation
- Razor default HTML encoding, as noted in 2.12.

#### CSRF / AntiForgeryToken
- POST actions protected with `[ValidateAntiForgeryToken]`, e.g.:
  - `Controllers/BorrowRecordsController.cs:84-85`
  - `Controllers/FavoritesController.cs:40-41, 56-57, 72-73`
  - `Controllers/FeedbackController.cs:63-64, 98-99, 151-152, 177-178`
  - `Controllers/RepairRequestsController.cs:100-102`
  - `Areas/Admin/Controllers/ToolsController.cs:95-96, 140-141, 196-197`
  - `Areas/Admin/Controllers/RepairSessionsController.cs:89-90, 123-124, 163-164`
  - `Areas/Admin/Controllers/LocationsController.cs:69-70, 97-98, 131-132`
  - `Areas/Admin/Controllers/AnnouncementsController.cs:72-73, 100-101, 132-133`
- Forms include anti-forgery tokens, e.g.:
  - `Views/RepairRequests/Create.cshtml:13-15`
  - `Views/BorrowRecords/Create.cshtml:30`
  - `Areas/Admin/Views/Tools/Create.cshtml:8`
  - `Areas/Admin/Views/RepairSessions/Create.cshtml:8`
  - `Areas/Admin/Views/Locations/Create.cshtml:8`

#### Authorization and access control
- User-only functionality via `[Authorize]`, e.g.:
  - `Controllers/BorrowRecordsController.cs:9`
  - `Controllers/FavoritesController.cs:9`
  - `Controllers/FeedbackController.cs:9`
  - `Controllers/RepairRequestsController.cs:20, 43, 67, 92, 100`
  - `Controllers/VolunteersController.cs:24, 32`
- Admin-only area via `[Authorize(Roles = "Administrator")]`, see section 2.8.

---

## 3. Additional architecture / OOP requirements mapping

### 3.1 Well-structured architecture and configured control flow — **fulfilled**
The project is separated into layers/folders:
- `Controllers/` and `Areas/Admin/Controllers/` → web layer
- `Services/Interfaces/` and `Services/Implementations/` → business logic layer
- `Data/` → persistence layer
- `ViewModels/` → UI contracts/view models
- `Views/` and `Areas/Admin/Views/` → presentation layer
- `RepairCircle.Tests/` → automated tests

Key wiring points:
- startup configuration in `Program.cs:9-75`
- service registration in `Services/ServiceCollectionExtensions.cs:7-24`

### 3.2 OOP principles — **fulfilled**
- **Encapsulation**: entity state and validation live inside model classes, e.g. `BorrowRecord.cs:7-52`, `RepairSession.cs:6-60`, `Location.cs:5-50`.
- **Inheritance**:
  - `ApplicationUser : IdentityUser` in `Data/Models/ApplicationUser.cs:5-13`
  - most domain models inherit from `BaseEntity` (e.g. `Tool.cs:7`, `RepairRequest.cs:7`, `RepairSession.cs:6`).
- **Abstraction / interfaces**:
  - service interfaces in `Services/Interfaces/*.cs`
  - e.g. `IRealtimeNotificationService.cs:3-10`
- **Polymorphism / DI substitution**:
  - interface-to-implementation mapping in `Services/ServiceCollectionExtensions.cs:11-21`

### 3.3 Exception handling — **fulfilled**
- global exception pipeline in `Program.cs:44-55`
- custom exception endpoints in `Controllers/HomeController.cs:30-76`
- startup seeding guarded by try/catch in `Data/Seed/ApplicationDbInitializer.cs:21-33`
- several list services return safe empty models instead of crashing the app when unexpected exceptions happen (example: `Services/Implementations/ToolService.cs:28-147`)

### 3.4 Strong cohesion / loose coupling — **fulfilled**
- Controllers depend on interfaces/services instead of embedding business logic.
- Example:
  - `Controllers/HomeController.cs:11-20`
  - `Controllers/RepairRequestsController.cs` calls `IRepairRequestService`
  - `Controllers/ToolsController.cs` calls `IToolService`
- Service registration centralised in `Services/ServiceCollectionExtensions.cs:7-24`.

### 3.5 Readable code and naming — **fulfilled**
- Consistent naming across layers (`ToolService`, `RepairRequestService`, `BorrowRecordService`, corresponding interfaces and view models).
- Root solution structure is clearly separated by responsibility (see section 3.1).

### 3.6 Good-looking and usable UI — **fulfilled**
- Styled public layout in `Views/Shared/_Layout.cshtml:11-63`.
- Styled admin layout in `Areas/Admin/Views/Shared/_AdminLayout.cshtml:13-61`.
- Styled login/register pages:
  - `Areas/Identity/Pages/Account/Login.cshtml:7-66`
  - `Areas/Identity/Pages/Account/Register.cshtml:7-71`
- Large custom stylesheet in `wwwroot/css/site.css` with cards, grids, forms, status badges, auth pages, admin theme, and responsive breakpoints.

### 3.7 Support all major modern browsers — **fulfilled by technology choice and CSS/HTML structure**
- Standard HTML5 layout and responsive viewport:
  - `Views/Shared/_Layout.cshtml:1-9`
  - `Areas/Admin/Views/Shared/_AdminLayout.cshtml:4-12`
- Standard ASP.NET Core MVC + Razor + CSS + vanilla JavaScript + Chart.js + SignalR JS CDN.

---

## 4. Bonus requirements mapping

### 4.1 SignalR communication — **implemented**
- Hub class: `Hubs/RepairCircleHub.cs:5-21`
- Hub mapped in `Program.cs:72`
- Realtime notification interface: `Services/Interfaces/IRealtimeNotificationService.cs:3-10`
- Realtime implementation: `Services/Implementations/RealtimeNotificationService.cs:7-46`
- Admin live notification client:
  - `Areas/Admin/Views/Shared/_AdminLayout.cshtml:23-30, 60-61`
  - `wwwroot/js/admin-notifications.js:1-42`
- Tool live availability client:
  - `Views/Tools/Details.cshtml:66-68, 115-127`
  - `wwwroot/js/realtime-tool-details.js:1-58`
- SignalR events are triggered from services:
  - `Services/Implementations/BorrowRecordService.cs:257-266`
  - `Services/Implementations/RepairRequestService.cs:195`

### 4.2 Image upload — **implemented**
- File service and validation:
  - `Services/Interfaces/IFileStorageService.cs:7-8`
  - `Services/Implementations/FileStorageService.cs:8-45, 47-76`
- Admin tool image upload:
  - form fields in `Areas/Admin/Views/Shared/_ToolFormPartial.cshtml:16-35`
  - controller handling in `Areas/Admin/Controllers/ToolsController.cs:104-117, 154-176`
- User repair-request image upload:
  - form fields in `Views/RepairRequests/Create.cshtml:56-67`
  - controller handling in `Controllers/RepairRequestsController.cs:105-119`

### 4.3 Admin dashboard charts — **implemented**
- Dashboard metrics and chart data service in `Services/Implementations/AdminDashboardService.cs:18-98`
- Chart.js rendering in `Areas/Admin/Views/Dashboard/Index.cshtml:50-80, 143-217`

### 4.4 QR-like / reference-code generation — **implemented**
- Generator class: `Common/ReferenceCodeGenerator.cs:1-11`
- Request reference on model: `Data/Models/RepairRequest.cs:28-31`
- Borrow reference on model: `Data/Models/BorrowRecord.cs:32-35`
- References created in services:
  - `Services/Implementations/RepairRequestService.cs:167-197`
  - `Services/Implementations/BorrowRecordService.cs:247-266`
- Displayed in views:
  - `Views/RepairRequests/Details.cshtml` (request reference + partial)
  - `Areas/Admin/Views/RepairRequests/Details.cshtml:40-55`
  - `Views/BorrowRecords/Details.cshtml:52`
  - partial `Views/Shared/_ReferenceCodePartial.cshtml`

### 4.5 Local storage — **implemented**
- Recently viewed tools: `wwwroot/js/recent-tools.js:1-100`
- Saved tool filters: `wwwroot/js/tool-filters-storage.js:1-109`
- Repair request draft autosave: `wwwroot/js/repair-request-draft.js:1-125`
- Wired into pages:
  - `Views/Tools/Details.cshtml:107-127`
  - `Views/Home/Index.cshtml`
  - `Views/Tools/Index.cshtml`
  - `Views/RepairRequests/Create.cshtml:17-23, 88-91`

### 4.6 Google Maps integration — **implemented**
- View model: `ViewModels/Common/MapEmbedViewModel.cs:3-46`
- Reusable partial: `Views/Shared/_LocationMapPartial.cshtml:1-32`
- Used on details pages, e.g.:
  - `Views/Tools/Details.cshtml:16-23, 64`
  - `Views/RepairRequests/Details.cshtml`
  - `Views/RepairSessions/Details.cshtml`
  - `Views/Home/Contact.cshtml`

### 4.7 AJAX partial updates — **implemented**
- AJAX favorites toggle:
  - `wwwroot/js/ajax-favorites.js:1-60`
  - `Controllers/FavoritesController.cs`
  - `Views/Tools/Details.cshtml:76-90, 126`
- AJAX feedback load/create/edit:
  - `wwwroot/js/repair-request-feedback.js:1-150`
  - `Views/Shared/_AjaxFeedbackFormPartial.cshtml`
  - `Controllers/FeedbackController.cs`

---

## 5. Testing requirement mapping

### 5.1 Separate unit-test project — **implemented**
- Test project file: `RepairCircle.Tests/RepairCircle.Tests.csproj:1-27`
- xUnit and test packages:
  - `RepairCircle.Tests/RepairCircle.Tests.csproj:10-21`
- In-memory EF Core package for isolated service tests:
  - `RepairCircle.Tests/RepairCircle.Tests.csproj:15`
- Coverage collector package:
  - `RepairCircle.Tests/RepairCircle.Tests.csproj:11-14`

### 5.2 Unit tests for logic/services — **implemented**
The test suite covers the business logic layer, not only controllers:
- `RepairCircle.Tests/ToolServiceTests.cs:3-43`
- `RepairCircle.Tests/FavoriteServiceTests.cs:3-57`
- `RepairCircle.Tests/BorrowRecordServiceTests.cs:3-67`
- `RepairCircle.Tests/RepairRequestServiceTests.cs:3-73`
- `RepairCircle.Tests/VolunteerAndSessionServiceTests.cs:3-65`
- `RepairCircle.Tests/FeedbackServiceTests.cs:3-51`
- shared test infrastructure:
  - `RepairCircle.Tests/TestDbFactory.cs`
  - `RepairCircle.Tests/FakeRealtimeNotificationService.cs`
  - `RepairCircle.Tests/GlobalUsings.cs`

### 5.3 “At least 65% of business logic” — **partially evidenced in code; percentage must be verified by running coverage**
The codebase clearly includes a service-layer test suite that targets the main business services.

However, the **exact percentage** (65%+) cannot be proven from static file inspection alone. The examiner should verify it by running:

```powershell

dotnet test RepairCircle.Tests/RepairCircle.Tests.csproj --collect:"XPlat Code Coverage"
```

Evidence that coverage tooling is already included:
- `RepairCircle.Tests/RepairCircle.Tests.csproj:11-14`
- `RUN-AND-TEST.txt:27` (coverage command is documented there)

---

## 6. Role logic currently implemented

### Guest
- Public browsing only via navigation in `Views/Shared/_Layout.cshtml:13-33`.
- No create/modify actions without authentication.

### Registered user
- Request repair: `Views/Shared/_Layout.cshtml:19-25` and `Controllers/RepairRequestsController.cs`.
- Own repairs only: `Views/Shared/_Layout.cshtml:21-24` and `Controllers/RepairRequestsController.cs` / `RepairRequestService.GetMineAsync`.
- Own borrowings only: `Views/Shared/_Layout.cshtml:22-24` and `Controllers/BorrowRecordsController.cs`.
- Favorites and feedback available through authenticated controllers.

### Admin
- Admin navigation in `Views/Shared/_Layout.cshtml:27-32`.
- Admin live notifications in `Areas/Admin/Views/Shared/_AdminLayout.cshtml:23-30`.
- Full CRUD/management controllers under `Areas/Admin/Controllers/`.

### Volunteer
- Volunteer role is seeded in `Data/Seed/ApplicationDbInitializer.cs:38-42, 59-63`, and volunteer profiles are seeded in `Data/Seed/ApplicationDbInitializer.cs:174-205`.
- Volunteer-specific data and browse pages exist, but the project does **not** currently enforce a separate volunteer-only area using `[Authorize(Roles = "Volunteer")]`.

---

