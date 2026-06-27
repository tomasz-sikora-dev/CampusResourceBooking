# CampusResourceBooking

CampusResourceBooking is a Blazor Server application for booking university campus resources such as lecture halls, labs, study rooms and equipment. The project is inspired by the structure and behavior of `InternshipManagementProgram`, but it uses a different business domain and intentionally separates UI, services, models and validation logic.

## Main features

- Student panel for creating, viewing and cancelling booking requests.
- Admin panel for reviewing, approving and rejecting requests.
- Resource catalogue with capacity, location, category and availability status.
- Booking status workflow: `Draft`, `Submitted`, `Approved`, `Rejected`, `Cancelled`, `Completed`.
- SQL Server database script with tables, seed data, views and stored procedures.
- PDF confirmation generator based on QuestPDF.
- Unit tests for validation and service behavior.

## Technology stack

- .NET 10 / Blazor Server interactive components
- MudBlazor
- Entity Framework Core SQL Server
- QuestPDF
- MSTest

## Default users

| Role | Email | Password |
| --- | --- | --- |
| Administrator | admin@campus.local | admin123 |
| Student | student@campus.local | student123 |

## Getting started

1. Create the database with `database/CampusResourceBooking.sql`.
2. Update the connection string in `CampusResourceBooking/appsettings.json`.
3. Run the application:

```bash
dotnet restore
dotnet run --project CampusResourceBooking/CampusResourceBooking.csproj
```

4. Run tests:

```bash
dotnet test CampusResourceBooking.Tests/CampusResourceBooking.Tests.csproj
```

## Clean code decisions

- Business rules live in services and validators, not in Razor pages.
- Models map the database; DTOs carry UI form data.
- Pages are thin: they call services and display results.
- Status display is centralized in `StatusInfoProvider`.
- PDF generation is isolated behind `BookingPdfService`.
- Database access is asynchronous and query projections use `AsNoTracking` where possible.
