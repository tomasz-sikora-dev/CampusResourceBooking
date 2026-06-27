IF DB_ID('CampusResourceBooking') IS NULL
BEGIN
    CREATE DATABASE CampusResourceBooking;
END
GO

USE CampusResourceBooking;
GO

IF OBJECT_ID('dbo.sp_UpdateBookingStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_UpdateBookingStatus;
IF OBJECT_ID('dbo.vResourceUsage', 'V') IS NOT NULL DROP VIEW dbo.vResourceUsage;
IF OBJECT_ID('dbo.vBookingStatistics', 'V') IS NOT NULL DROP VIEW dbo.vBookingStatistics;
IF OBJECT_ID('dbo.vBookingFull', 'V') IS NOT NULL DROP VIEW dbo.vBookingFull;
IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID('dbo.Resources', 'U') IS NOT NULL DROP TABLE dbo.Resources;
IF OBJECT_ID('dbo.ResourceCategories', 'U') IS NOT NULL DROP TABLE dbo.ResourceCategories;
IF OBJECT_ID('dbo.CampusUsers', 'U') IS NOT NULL DROP TABLE dbo.CampusUsers;
GO

CREATE TABLE dbo.CampusUsers
(
    CampusUserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CampusUsers PRIMARY KEY,
    FirstName NVARCHAR(80) NOT NULL,
    LastName NVARCHAR(80) NOT NULL,
    Email NVARCHAR(160) NOT NULL CONSTRAINT UQ_CampusUsers_Email UNIQUE,
    PasswordHash NVARCHAR(128) NOT NULL,
    Role NVARCHAR(40) NOT NULL,
    StudentNumber NVARCHAR(80) NULL,
    Department NVARCHAR(120) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_CampusUsers_IsActive DEFAULT 1
);
GO

CREATE TABLE dbo.ResourceCategories
(
    ResourceCategoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ResourceCategories PRIMARY KEY,
    Name NVARCHAR(80) NOT NULL CONSTRAINT UQ_ResourceCategories_Name UNIQUE,
    Description NVARCHAR(250) NULL
);
GO

CREATE TABLE dbo.Resources
(
    ResourceId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Resources PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Building NVARCHAR(80) NOT NULL,
    RoomNumber NVARCHAR(30) NOT NULL,
    Capacity INT NOT NULL,
    RequiresApproval BIT NOT NULL CONSTRAINT DF_Resources_RequiresApproval DEFAULT 1,
    IsActive BIT NOT NULL CONSTRAINT DF_Resources_IsActive DEFAULT 1,
    Equipment NVARCHAR(500) NULL,
    ResourceCategoryId INT NOT NULL,
    CONSTRAINT FK_Resources_ResourceCategories FOREIGN KEY(ResourceCategoryId) REFERENCES dbo.ResourceCategories(ResourceCategoryId),
    CONSTRAINT CK_Resources_Capacity CHECK (Capacity > 0)
);
GO

CREATE TABLE dbo.Bookings
(
    BookingId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Bookings PRIMARY KEY,
    ResourceId INT NOT NULL,
    RequestedByUserId INT NOT NULL,
    DecidedByUserId INT NULL,
    StartsAt DATETIME2 NOT NULL,
    EndsAt DATETIME2 NOT NULL,
    Purpose NVARCHAR(200) NOT NULL,
    Notes NVARCHAR(1000) NULL,
    DecisionComment NVARCHAR(1000) NULL,
    Status NVARCHAR(40) NOT NULL CONSTRAINT DF_Bookings_Status DEFAULT 'Submitted',
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT SYSUTCDATETIME(),
    DecidedAt DATETIME2 NULL,
    CONSTRAINT FK_Bookings_Resources FOREIGN KEY(ResourceId) REFERENCES dbo.Resources(ResourceId),
    CONSTRAINT FK_Bookings_RequestedBy FOREIGN KEY(RequestedByUserId) REFERENCES dbo.CampusUsers(CampusUserId),
    CONSTRAINT FK_Bookings_DecidedBy FOREIGN KEY(DecidedByUserId) REFERENCES dbo.CampusUsers(CampusUserId),
    CONSTRAINT CK_Bookings_TimeRange CHECK (EndsAt > StartsAt),
    CONSTRAINT CK_Bookings_Status CHECK (Status IN ('Draft', 'Submitted', 'Approved', 'Rejected', 'Cancelled', 'Completed'))
);
GO

CREATE INDEX IX_Bookings_Resource_Time ON dbo.Bookings(ResourceId, StartsAt, EndsAt);
CREATE INDEX IX_Bookings_Status ON dbo.Bookings(Status);
GO

INSERT INTO dbo.CampusUsers(FirstName, LastName, Email, PasswordHash, Role, StudentNumber, Department)
VALUES
('Alicja', 'Admin', 'admin@campus.local', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Administrator', NULL, 'Administracja'),
('Jan', 'Student', 'student@campus.local', '703b0a3d6ad75b649a28adde7d83c6251da457549263bc7ff45ec709b0a8448b', 'Student', 'S12345', 'Informatyka');
GO

INSERT INTO dbo.ResourceCategories(Name, Description)
VALUES
('Sala dydaktyczna', 'Sale do zajęć i spotkań grupowych'),
('Laboratorium', 'Pracownie specjalistyczne wymagające akceptacji'),
('Pokój pracy cichej', 'Pokoje do pracy indywidualnej lub małych zespołów'),
('Sprzęt', 'Przenośny sprzęt kampusowy');
GO

INSERT INTO dbo.Resources(Name, Building, RoomNumber, Capacity, RequiresApproval, IsActive, Equipment, ResourceCategoryId)
VALUES
('Aula Kopernika', 'Budynek A', 'A-101', 120, 1, 1, 'Projektor, system audio, mikrofony', 1),
('Laboratorium AI', 'Budynek B', 'B-204', 24, 1, 1, 'Stacje GPU, tablica interaktywna', 2),
('Pokój pracy zespołowej', 'Biblioteka', 'L-12', 8, 0, 1, 'Monitor, whiteboard', 3),
('Zestaw VR', 'Centrum Medialne', 'M-02', 4, 1, 1, '4 gogle VR, laptop', 4);
GO

CREATE OR ALTER VIEW dbo.vBookingFull
AS
SELECT
    b.BookingId,
    r.Name AS ResourceName,
    c.Name AS CategoryName,
    r.Building,
    r.RoomNumber,
    r.Capacity,
    CONCAT(u.FirstName, ' ', u.LastName) AS RequestedBy,
    u.Email AS RequesterEmail,
    u.Department,
    b.StartsAt,
    b.EndsAt,
    b.Purpose,
    b.Status,
    b.DecisionComment,
    b.CreatedAt
FROM dbo.Bookings b
JOIN dbo.Resources r ON r.ResourceId = b.ResourceId
JOIN dbo.ResourceCategories c ON c.ResourceCategoryId = r.ResourceCategoryId
JOIN dbo.CampusUsers u ON u.CampusUserId = b.RequestedByUserId;
GO

CREATE OR ALTER VIEW dbo.vBookingStatistics
AS
SELECT
    Status,
    COUNT(*) AS TotalBookings,
    COUNT(DISTINCT ResourceId) AS UniqueResources
FROM dbo.Bookings
GROUP BY Status;
GO

CREATE OR ALTER VIEW dbo.vResourceUsage
AS
SELECT
    r.ResourceId,
    r.Name AS ResourceName,
    c.Name AS CategoryName,
    SUM(CASE WHEN b.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedBookings,
    SUM(CASE WHEN b.Status = 'Submitted' THEN 1 ELSE 0 END) AS PendingBookings,
    MIN(CASE WHEN b.Status IN ('Approved', 'Submitted') AND b.StartsAt >= SYSUTCDATETIME() THEN b.StartsAt ELSE NULL END) AS NearestBooking
FROM dbo.Resources r
JOIN dbo.ResourceCategories c ON c.ResourceCategoryId = r.ResourceCategoryId
LEFT JOIN dbo.Bookings b ON b.ResourceId = r.ResourceId
GROUP BY r.ResourceId, r.Name, c.Name;
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateBookingStatus
    @BookingId INT,
    @Status NVARCHAR(40),
    @DecidedByUserId INT,
    @DecisionComment NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Bookings
    SET Status = @Status,
        DecidedByUserId = @DecidedByUserId,
        DecisionComment = @DecisionComment,
        DecidedAt = SYSUTCDATETIME()
    WHERE BookingId = @BookingId
      AND Status = 'Submitted';
END;
GO
