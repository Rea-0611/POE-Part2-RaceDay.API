-- ============================================
-- Database: RaceDay
-- Author: Pontsho Sedumedi (ST10483202)
-- Description: Creates the full database schema for the RaceDay system
-- ============================================

-- ============================================
-- STEP 1: Clean Up Existing Tables
-- ============================================

USE master;

-- Drop the database if it exists (clean slate)
IF DB_ID('RaceDay') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDay SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDay;
END

-- ============================================
-- STEP 2: Create Database
-- ============================================

CREATE DATABASE RaceDay;

-- ============================================
-- STEP 3: Use Database
-- ============================================

USE RaceDay;

-- ============================================
-- STEP 4: Drop Existing Tables (if any)
-- ============================================
-- Drop child tables first to avoid FK conflicts

IF OBJECT_ID('Results', 'U') IS NOT NULL DROP TABLE Results;
IF OBJECT_ID('Enrolments', 'U') IS NOT NULL DROP TABLE Enrolments;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Events', 'U') IS NOT NULL DROP TABLE Events;
IF OBJECT_ID('Organisers', 'U') IS NOT NULL DROP TABLE Organisers;
IF OBJECT_ID('Participants', 'U') IS NOT NULL DROP TABLE Participants;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;

-- ============================================
-- STEP 5: Create Tables
-- ============================================

-- 5.1 Users (base table)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant'))
);

-- 5.2 Organisers (extends Users)
CREATE TABLE Organisers (
    OrganiserId INT PRIMARY KEY,
    OrganisationName NVARCHAR(100),
    CONSTRAINT FK_Organisers_Users FOREIGN KEY (OrganiserId)
        REFERENCES Users(UserId) ON DELETE CASCADE
);

-- 5.3 Participants (extends Users)
CREATE TABLE Participants (
    ParticipantId INT PRIMARY KEY,
    DateOfBirth DATE,
    ContactNumber NVARCHAR(20),
    CONSTRAINT FK_Participants_Users FOREIGN KEY (ParticipantId)
        REFERENCES Users(UserId) ON DELETE CASCADE
);

-- 5.4 Events (depends on Organisers)
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserId INT NOT NULL,
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Distance NVARCHAR(50),
    EventType NVARCHAR(20) NOT NULL DEFAULT 'Run'
        CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    EventStatus NVARCHAR(20) DEFAULT 'Draft'
        CHECK (EventStatus IN ('Draft', 'Open', 'Closed', 'Finished')),
    CONSTRAINT FK_Events_Organisers FOREIGN KEY (OrganiserId)
        REFERENCES Organisers(OrganiserId) ON DELETE CASCADE
);

-- 5.5 Categories (depends on Events)
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    CategoryName NVARCHAR(50) NOT NULL,
    CategoryType NVARCHAR(20) NOT NULL DEFAULT 'Distance'
        CHECK (CategoryType IN ('Age', 'Distance')),
    EntryFee DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventId)
        REFERENCES Events(EventId) ON DELETE CASCADE
);

-- 5.6 Enrolments (depends on Participants AND Categories)
-- FK to Categories uses NO ACTION to prevent multiple cascade paths
CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL,
    CategoryId INT NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending'
        CHECK (PaymentStatus IN ('Pending', 'Paid')),
    CONSTRAINT FK_Enrolments_Participants FOREIGN KEY (ParticipantId)
        REFERENCES Participants(ParticipantId) ON DELETE CASCADE,
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryId)
        REFERENCES Categories(CategoryId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Enrolment_Unique UNIQUE (ParticipantId, CategoryId)
);

-- 5.7 Results (depends on Enrolments)
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL UNIQUE,
    FinishTime TIME(3),
    OverallPosition INT,
    CategoryPosition INT,
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentId)
        REFERENCES Enrolments(EnrolmentId) ON DELETE CASCADE
);

-- ============================================
-- STEP 6: Insert Sample Data
-- ============================================

-- 6.1 Insert Users
INSERT INTO Users (Email, PasswordHash, FullName, Role) VALUES
('john.doe@raceday.com', 'hashed_password_1', 'John Doe', 'Organiser'),
('jane.smith@raceday.com', 'hashed_password_2', 'Jane Smith', 'Organiser'),
('alice.runner@example.com', 'hashed_password_3', 'Alice Runner', 'Participant'),
('bob.walker@example.com', 'hashed_password_4', 'Bob Walker', 'Participant');

-- 6.2 Insert Organisers
INSERT INTO Organisers (OrganiserId, OrganisationName) VALUES
(1, 'Durban Running Club'),
(2, 'Cape Town Cycle Events');

-- 6.3 Insert Participants
INSERT INTO Participants (ParticipantId, DateOfBirth, ContactNumber) VALUES
(3, '1990-05-15', '0821234567'),
(4, '1985-11-20', '0839876543');

-- 6.4 Insert Events
INSERT INTO Events (OrganiserId, EventName, EventDate, Location, Description, Distance, EventType, EventStatus) VALUES
(1, 'Durban City Marathon', '2026-10-15 06:00:00', 'Durban, KZN', 'A scenic marathon along the Durban coastline.', '42.2km', 'Run', 'Open'),
(1, 'Durban 10km Walk', '2026-11-05 08:00:00', 'Durban, KZN', 'A family-friendly 10km walk through the city.', '10km', 'Walk', 'Open'),
(2, 'Cape Town Cycle Tour', '2026-03-08 07:00:00', 'Cape Town, WC', 'The world-famous Argus Cycle Tour.', '109km', 'Cycle', 'Open');

-- 6.5 Insert Categories
INSERT INTO Categories (EventId, CategoryName, CategoryType, EntryFee) VALUES
(1, 'Full Marathon (42.2km)', 'Distance', 250.00),
(1, 'Half Marathon (21.1km)', 'Distance', 150.00),
(1, 'Senior Men (40+)', 'Age', 200.00),
(2, '10km Open Walk', 'Distance', 50.00),
(2, 'Junior Walk (Under 20)', 'Age', 30.00),
(3, 'Elite Cyclist', 'Age', 400.00),
(3, 'Amateur Cyclist', 'Distance', 300.00);

-- 6.6 Insert Enrolments
INSERT INTO Enrolments (ParticipantId, CategoryId, PaymentStatus) VALUES
(3, 1, 'Paid'),
(3, 4, 'Pending'),
(4, 7, 'Paid');

-- 6.7 Insert Results
INSERT INTO Results (EnrolmentId, FinishTime, OverallPosition, CategoryPosition) VALUES
(1, '03:45:30.123', 15, 2),
(3, '04:12:15.000', 78, 4);

-- ============================================
-- STEP 7: Verification
-- ============================================

-- 7.1 Show each table separately
SELECT '========== USERS =========='  AS Section;
SELECT * FROM Users;

SELECT '========== ORGANISERS ==========' AS Section;
SELECT * FROM Organisers;

SELECT '========== PARTICIPANTS ==========' AS Section;
SELECT * FROM Participants;

SELECT '========== EVENTS ==========' AS Section;
SELECT * FROM Events;

SELECT '========== CATEGORIES ==========' AS Section;
SELECT * FROM Categories;

SELECT '========== ENROLMENTS ==========' AS Section;
SELECT * FROM Enrolments;

SELECT '========== RESULTS ==========' AS Section;
SELECT * FROM Results;

-- 7.2 Events with organiser details
SELECT '========== EVENTS WITH ORGANISERS ==========' AS Section;
SELECT
    e.EventId,
    e.EventName,
    e.EventDate,
    e.Location,
    e.Distance,
    e.EventType,
    e.EventStatus,
    u.FullName AS OrganiserName,
    o.OrganisationName
FROM Events e
JOIN Organisers o ON e.OrganiserId = o.OrganiserId
JOIN Users u ON o.OrganiserId = u.UserId;

-- 7.3 Categories with event details
SELECT '========== CATEGORIES WITH EVENTS ==========' AS Section;
SELECT
    c.CategoryId,
    c.CategoryName,
    c.CategoryType,
    c.EntryFee,
    e.EventName
FROM Categories c
JOIN Events e ON c.EventId = e.EventId;

-- 7.4 Participant enrolments (full details)
SELECT '========== PARTICIPANT ENROLMENTS ==========' AS Section;
SELECT
    en.EnrolmentId,
    u.FullName AS ParticipantName,
    e.EventName,
    c.CategoryName,
    c.CategoryType,
    en.EnrolmentDate,
    en.PaymentStatus
FROM Enrolments en
JOIN Participants p ON en.ParticipantId = p.ParticipantId
JOIN Users u ON p.ParticipantId = u.UserId
JOIN Categories c ON en.CategoryId = c.CategoryId
JOIN Events e ON c.EventId = e.EventId;

-- 7.5 Results with participant details
SELECT '========== RESULTS WITH PARTICIPANTS ==========' AS Section;
SELECT
    r.ResultId,
    u.FullName AS ParticipantName,
    c.CategoryName,
    r.FinishTime,
    r.OverallPosition,
    r.CategoryPosition
FROM Results r
JOIN Enrolments en ON r.EnrolmentId = en.EnrolmentId
JOIN Participants p ON en.ParticipantId = p.ParticipantId
JOIN Users u ON p.ParticipantId = u.UserId
JOIN Categories c ON en.CategoryId = c.CategoryId;

-- 7.6 Table row counts
SELECT '========== TABLE ROW COUNTS ==========' AS Section;
SELECT 'Users' AS TableName, COUNT(*) AS String FROM Users
UNION ALL SELECT 'Organisers', COUNT(*) FROM Organisers
UNION ALL SELECT 'Participants', COUNT(*) FROM Participants
UNION ALL SELECT 'Events', COUNT(*) FROM Events
UNION ALL SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL SELECT 'Enrolments', COUNT(*) FROM Enrolments
UNION ALL SELECT 'Results', COUNT(*) FROM Results;

select*from Users;