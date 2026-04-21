# RepairCircle

**RepairCircle** is an ASP.NET Core MVC web application for managing community repair requests, tool lending, volunteer participation, and administrative coordination.

It is designed as a practical civic/community platform where users can request help repairing broken items, browse and borrow tools, save favorites, join the volunteer community, and interact with a repair-oriented ecosystem. Administrators manage the platform through a dedicated admin area with CRUD operations, dashboard reporting, notifications, and moderation workflows.

---

## Table of Contents

1. [Project Concept](#project-concept)
2. [Goals and Motivation](#goals-and-motivation)
3. [Main Functionalities](#main-functionalities)
4. [User Roles and Permissions](#user-roles-and-permissions)
5. [Application Architecture](#application-architecture)
6. [Project Structure](#project-structure)
7. [Domain Models](#domain-models)
8. [Services Layer](#services-layer)
9. [Admin Area](#admin-area)
10. [Authentication and Authorization](#authentication-and-authorization)
11. [Validation and Error Handling](#validation-and-error-handling)
12. [Search, Filtering, Pagination, and UX Enhancements](#search-filtering-pagination-and-ux-enhancements)
13. [Bonus Features](#bonus-features)
14. [Database Design and Seeding](#database-design-and-seeding)
15. [Setup and Run Instructions](#setup-and-run-instructions)
16. [Testing and Test Coverage](#testing-and-test-coverage)
17. [Deployment Notes](#deployment-notes)
18. [Design Decisions](#design-decisions)
19. [Known Limitations / Future Improvements](#known-limitations--future-improvements)
20. [Optional Screenshots Section](#optional-screenshots-section)
21. [Conclusion](#conclusion)

---

## Project Concept

RepairCircle is a community-focused web platform with two central purposes:

1. **Repair coordination** — users can submit repair requests for broken household or personal items.
2. **Tool lending** — users can browse and request community tools for short-term use.

The platform is inspired by repair cafés, makerspaces, community workshops, and local sustainability initiatives. It promotes reuse, practical problem solving, and collaborative support.

Typical use cases include:
- A user requests repair help for a broken speaker, bicycle, zipper, or appliance.
- A user borrows a tool such as a drill or sewing machine.
- A volunteer shares skills and joins repair efforts.
- An administrator monitors activity, manages sessions and tools, and keeps the system organized.

---

## Goals and Motivation

The project was created to demonstrate a complete ASP.NET Core MVC application that includes:

- layered architecture
- clean separation of concerns
- authentication and authorization
- database modeling with Entity Framework Core
- admin management workflows
- validation and security measures
- unit tests for business logic
- responsive and usable user interface
- practical bonus features such as SignalR, local image upload, local storage helpers, charts, Google Maps integration, and AJAX interactions

The application also addresses a realistic social problem: reducing waste by promoting repair and responsible tool sharing.

---

## Main Functionalities

### Public / Visitor Features
- Browse tools
- Browse repair sessions
- Browse public volunteer profiles
- Browse public repair-related information
- View location-based information and maps
- Register and log in

### Registered User Features
- Submit repair requests
- View only their own repair requests
- Borrow tools
- View only their own borrow records
- Save tools as favorites
- Leave feedback on repair requests
- Apply to become a volunteer
- Upload images to repair requests

### Admin Features
- View system dashboard
- Manage tools
- Manage repair requests
- Assign volunteers to repair requests
- Assign repair sessions to requests
- Manage repair sessions
- Manage locations
- Manage announcements
- View all users overview
- View all borrowings
- Receive live SignalR notifications

---

## User Roles and Permissions

### 1. Guest (Unauthenticated Visitor)
A guest can:
- view the home page
- browse tools
- browse public repair sessions
- browse volunteer information
- view maps and contact/location information
- register or log in

A guest cannot:
- create repair requests
- borrow tools
- add favorites
- view personal data pages
- access admin functionality

### 2. Registered User
A registered user can:
- create repair requests
- view only their own repair requests
- request / create borrowing records for tools
- view only their own borrowing records
- save tools to favorites
- leave feedback
- create a volunteer profile request
- upload an image for a repair request

### 3. Volunteer
The project includes a volunteer role and volunteer profile functionality. Volunteers are represented with skills, experience level, and approval state.

Current volunteer functionality includes:
- being listed in volunteer-related pages
- being assignable to repair requests and sessions by an administrator
- owning a volunteer profile with skills and metadata

### 4. Administrator
An administrator can:
- access the Admin area
- manage all tools
- manage all repair requests
- manage all borrowings
- manage sessions and locations
- manage announcements
- view a summary dashboard with charts
- monitor the platform and workflow state

---

## Application Architecture

The project follows a layered ASP.NET Core MVC architecture.

### Presentation Layer
Contains:
- MVC controllers
- Razor views
- Razor partial views
- layout files
- static assets (CSS, JavaScript, images)

This layer is responsible for handling requests, rendering UI, and orchestrating calls to services.

### Business Logic Layer
Contains:
- service interfaces
- service implementations
- application-specific operations and rules

This layer handles business logic such as:
- creating borrow records
- filtering data
- role-aware visibility
- favorite management
- volunteer/profile logic
- request assignment and state changes

### Data Access Layer
Contains:
- `ApplicationDbContext`
- entity models
- EF Core configuration
- migrations
- data seeding

This layer handles database persistence through Entity Framework Core.

### Shared / Common Concerns
Contains:
- enums
- helper utilities
- reference generation
- constants
- view models for structured UI communication

---

## Project Structure

A simplified structure is shown below:

```text
RepairCircle/
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       └── Views/
├── Controllers/
├── Data/
│   ├── Models/
│   ├── Enums/
│   ├── Seed/
│   ├── Migrations/
│   └── ApplicationDbContext.cs
├── Hubs/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── ViewModels/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── uploads/
├── RepairCircle.Tests/
├── Program.cs
└── appsettings.json
```

---

## Domain Models

The project includes more than the required minimum number of entity models.

### Core models
- `ApplicationUser`
- `Tool`
- `ToolCategory`
- `Location`
- `RepairRequest`
- `RepairSession`
- `VolunteerProfile`
- `Skill`
- `BorrowRecord`
- `Favorite`
- `Feedback`
- `Announcement`

### Examples of relationships
- A `Tool` belongs to one `ToolCategory` and one `Location`
- A `BorrowRecord` belongs to one `Tool` and one `ApplicationUser`
- A `RepairRequest` belongs to one user, one location, and can optionally be assigned to a volunteer and a session
- A `VolunteerProfile` belongs to one `ApplicationUser`
- A volunteer can have many skills
- A repair session can involve many volunteers
- A user can have many favorites and many borrow records

---

## Services Layer

The project uses a service layer to keep controllers thin and business logic organized.

### Main services
- `IHomeService`
- `IToolService`
- `IRepairRequestService`
- `IRepairSessionService`
- `IBorrowRecordService`
- `IFavoriteService`
- `IFeedbackService`
- `IVolunteerService`
- `IAdminDashboardService`
- `IRealtimeNotificationService`
- `IFileStorageService`

### Typical responsibilities
- querying and shaping list/detail data
- applying role-aware visibility rules
- creating and updating business records
- reducing code duplication across controllers
- performing async database operations through EF Core
- raising SignalR notifications
- handling local file upload for images

---

## Admin Area

The admin functionality is separated using an MVC Area.

### Admin pages include
- Dashboard
- Tools management
- Repair Requests management
- Repair Sessions management
- Locations management
- Announcements management
- Users overview
- Borrowing overview

### Admin-specific features
- richer dashboard with charts and metrics
- live update panel using SignalR
- editing and assignment workflows
- separate admin layout and sidebar

---

## Authentication and Authorization

The project uses **ASP.NET Core Identity**.

### Identity features in use
- registration
- login
- logout
- user role support
- role-restricted admin access

### Roles
- `User`
- `Volunteer`
- `Administrator`

### Authorization examples
- admin area is restricted to administrators
- repair creation requires login
- borrowing requires login
- users can access only their own personal data pages

### UI decisions
The login/register pages are styled custom screens focused on local authentication only. External authentication was removed from the visible flow to keep the user experience clean and relevant to the project requirements.

---

## Validation and Error Handling

The application applies validation at multiple levels.

### Data annotations
Examples include:
- `[Required]`
- `[StringLength]`
- `[Range]`
- specific metadata for URLs and date/time fields

### Server-side validation examples
- preventing duplicate favorites
- preventing duplicate feedback for the same request by the same user
- checking ownership before showing personal details
- validating borrowing rules and dates
- validating image uploads

### Database constraints
The database layer includes examples such as:
- unique indexes
- check constraints for valid numeric/date combinations
- relationship constraints and delete behaviors

### Error handling
- custom 404 page
- custom 500 page
- access denied page
- safer list rendering and null-safe UI paths

---

## Search, Filtering, Pagination, and UX Enhancements

The application includes:

### Search and filtering
- tool search/filtering
- volunteer filtering
- repair session filtering
- admin search/filtering for managed lists

### Pagination
Pagination is included across several major list screens to keep pages manageable.

### UI enhancements
- built-in images for seeded content
- placeholders when user/admin content has no image
- improved login/register styling
- framed/tinted navigation and cards
- map embeds and links
- AJAX partial updates in relevant pages

---

## Bonus Features

The project implements multiple bonus features.

### 1. SignalR
Used for real-time admin notifications and selected live updates.

### 2. Image Upload
Users and admins can upload local images where applicable.

### 3. Dashboard Charts
The admin dashboard includes chart-based reporting.

### 4. QR-like / Reference Codes
Borrowing and repair operations include reference-style identifiers.

### 5. Local Storage
Used for browser-side helpers such as recently viewed items and saved filters/drafts.

### 6. Google Maps Integration
Locations can be opened through embedded or linked maps.
Coordinates entered by an admin are formatted and preferred exactly when available.

### 7. AJAX Partial Updates
Certain interactions are updated asynchronously without full page refresh.

---

## Database Design and Seeding

### Database provider
- Microsoft SQL Server / LocalDB

### ORM
- Entity Framework Core

### Seeding
The project seeds:
- roles
- an admin account
- volunteer users
- standard users
- sample tools
- tool categories
- sample repair requests
- repair sessions
- locations
- announcements
- feedback / favorites / borrow records where relevant

### Built-in users
#### Admin
- `admin@repaircircle.local` / `Admin123!`

#### Volunteers
- `maria.ivanova@repaircircle.local` / `Volunteer123!`
- `nikolay.petrov@repaircircle.local` / `Volunteer123!`

#### Standard users
- `elena.georgieva@repaircircle.local` / `User123!`
- `georgi.dimitrov@repaircircle.local` / `User123!`

### Built-in images
The project includes local image assets for seeded tools and repair requests, in addition to support for external image links and local upload.

---

## Setup and Run Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB
- Visual Studio 2022/2026 or terminal with `dotnet` CLI

### Recommended clean start
From the project folder:

```powershell
dotnet clean
Remove-Item -Recurse -Force .\bin, .\obj -ErrorAction SilentlyContinue
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

### Local URLs
Open the app at:
- `https://localhost:7228`
- `http://localhost:5107`

### Fresh reset (optional)
If you want a guaranteed clean database for the current package:

```powershell
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

### Important note
Different project packages may use different database names. Old users, requests, or borrowings can remain if an existing LocalDB database is reused. For a true fresh start, either:
- use the package’s fresh database name
- drop the current database before running
- use an incognito browser window to avoid old login cookies/local storage

---

## Testing and Test Coverage

A separate test project is included:
- `RepairCircle.Tests`

### Covered areas
The tests focus on service-layer business logic, which is the most appropriate place for the assignment’s coverage requirement.

### Tested service categories
- tools
- favorites
- feedback
- repair requests
- borrow records
- volunteers
- sessions

### Running tests
Basic run:

```powershell
dotnet test RepairCircle.Tests/RepairCircle.Tests.csproj
```

Detailed output:

```powershell
dotnet test RepairCircle.Tests/RepairCircle.Tests.csproj --logger "console;verbosity=detailed"
```

Coverage run:

```powershell
dotnet test RepairCircle.Tests/RepairCircle.Tests.csproj --collect:"XPlat Code Coverage"
```

### Test strategy
The tests are designed to validate business logic rather than UI rendering.
Typical assertions include:
- ownership logic
- favorite add/remove behavior
- borrowing record creation behavior
- repair request creation / filtering behavior
- volunteer and session service behavior

---

## Deployment Notes

The project is primarily configured for local development and academic demonstration.

### For local deployment / demo
- use LocalDB
- apply migrations
- run via `dotnet run` or Visual Studio

### For production-style deployment (future)
Potential steps would include:
- switch from LocalDB to SQL Server / Azure SQL
- configure persistent static file storage for uploads
- configure a real email sender if password reset is needed
- configure HTTPS and environment-specific settings
- reduce verbose development logging
- add production-grade exception handling and monitoring

### Password reset
Password reset/email-based account recovery is not fully configured with a real email delivery provider in the current version.

---

## Design Decisions

### 1. MVC + Services
The project uses ASP.NET Core MVC with a service layer because it offers a strong balance between:
- academic clarity
- maintainability
- testability
- familiarity in enterprise .NET development

### 2. Identity for user management
ASP.NET Core Identity was chosen for secure role-based authentication instead of a custom authentication system.

### 3. Admin Area
An MVC Area was used to separate administration from the public app, keeping the architecture cleaner.

### 4. EF Core + SQL Server
This combination was chosen because it fits the assignment requirements and provides strong tooling support.

### 5. Service-layer unit tests
Business logic lives in services, so test coverage is concentrated there rather than in controllers/views.

### 6. Fresh database names in packaged iterations
Different project iterations may use fresh database names to prevent local testing data from leaking across versions during development.

---

## Known Limitations / Future Improvements

Even though the project is functional, several areas could be improved further.

### Current limitations
- Volunteer role can be expanded into a stronger permission model
- Password reset email delivery is not configured with a real email provider
- Some admin/user management workflows could be deeper
- UI polish can always be extended further
- Screenshots are optional and may need to be added manually in final documentation

### Future improvements
- full volunteer dashboard
- richer moderation workflows
- more advanced analytics
- export/reporting functionality
- email notifications
- calendar integration
- cloud file storage
- stronger deployment profile documentation

---

## Optional Screenshots Section


Recommended screenshots (not all shown):
- Home page
- Login page
- Register page
- Tools page
- Tool details page
- Repair request creation page
- My Repairs page
- Admin dashboard
- Admin tools page
- SignalR live admin update example
- Maps integration example
- Image upload example

Example screenshot section format:

## Screenshots
### Home Page
[Home Page](https://github.com/vy-softuni/ASPNETProject/blob/main/RepairCircle/docs/screenshots/home-page.png)
### Admin Dashboard
[Admin Dashboard](https://github.com/vy-softuni/ASPNETProject/blob/main/RepairCircle/docs/screenshots/admin-dashboard.png)


---

## Conclusion

RepairCircle is a full ASP.NET Core MVC community platform that combines repair coordination, tool lending, volunteer participation, and admin oversight in a structured, layered web application.

It demonstrates:
- MVC architecture
- role-based access control
- EF Core persistence
- seeded data
- validation and security practices
- admin workflows
- testing of business logic
- multiple practical bonus features

## Project requirements compliance

more details in the REQUIREMENTS-COMPLIANCE.md file in the docs folder.


