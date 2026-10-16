ReadMe File for Part 1 (updated)

📋 Overview
RaceDay is a comprehensive web-based event management platform designed specifically for the South African road running, walking, and cycling community. The system bridges the gap between event organisers and participants by replacing outdated paper-based registrations, spreadsheets, and disconnected communication channels with a modern, integrated digital solution.
From iconic events like the Comrades Marathon and Cape Town Cycle Tour to community park runs and charity cycling events, RaceDay streamlines the entire event lifecycle - from creation and registration to result management and participant tracking.
________________________________________
👥 User Roles
The system supports two distinct user roles with specific permissions:
🏢 Organiser
Event organisers have full control over their events and can:
•	Create, edit, and delete events
•	Define age or distance categories for each event
•	Capture and update participant results (finish times and positions)
•	View all enrolments for their events
•	Update participant payment statuses
•	Manage event status (Draft, Open, Closed, Finished)
🏃 Participant
Race participants can engage with the platform through:
•	Account creation and profile management
•	Browsing upcoming events
•	Entering events by selecting a category
•	Viewing their enrolment history
•	Tracking personal performance results
•	Checking event details including date, location, distance, and type
________________________________________
📁 Project Structure
text
RaceDay/
├── docs/
│   ├── RaceDay_ERD.png                # Entity Relationship Diagram
│   ├── endpoint-plan.md               # API Endpoint Plan
│   └── RaceDay_Database_Creation.sql  # SQL Database Script
├── .github/workflows/
│   └── validate-docs.yml              # CI/CD Validation
├── README.md
└── .gitignore
________________________________________
📊 Part 1 Deliverables
1. Entity Relationship Diagram (ERD)
File: docs/RaceDay_ERD.png
The ERD provides a visual blueprint of the database structure showing all 7 entities, their attributes, and the relationships between them.
Entity	Description
Users	Base table storing authentication and profile information for all users
Organisers	Extends Users with organisation-specific details
Participants	Extends Users with participant-specific details
Events	Stores event information including date, location, distance, type, and status
Categories	Defines age or distance race options within an event
Enrolments	Records when a participant signs up for a specific category
Results	Stores race results (finish time and positions) linked to enrolments
Key Features:
•	Clear primary and foreign key indicators
•	Cardinality for all relationships (one-to-many, one-to-one)
•	All attributes for each entity
•	User role inheritance (Organiser and Participant from User base)
________________________________________
2. API Endpoint Plan
File: docs/endpoint-plan.md
The API endpoint plan provides a comprehensive table of all 21 RESTful endpoints the system will expose. This serves as the blueprint for Part 2 implementation.
Category	Purpose	Endpoints
Authentication	User registration and login	2
Profile	View and update user profiles	2
Events	CRUD operations for events	5
Categories	Manage event categories	4
Enrolments	Participant registration and management	4
Results	Capture and manage race results	4
Total		21
Specifications per endpoint:
•	HTTP Method (GET, POST, PUT, DELETE)
•	Route (e.g., /api/events, /api/auth/login)
•	Description of functionality
•	Required Role (Public, Participant, Organiser)
•	Request Body (JSON format with examples)
•	Expected Response (success and failure scenarios with HTTP status codes)
________________________________________
3. SQL Database Script
File: docs/RaceDay_Database_Creation.sql
The SQL script creates the complete database schema in SQL Server Management Studio (SSMS). It includes all tables, constraints, and sample data needed to test the system.
Includes:
•	CREATE TABLE statements for all 7 entities
•	Primary keys for unique identification
•	Foreign keys for relationships between tables
•	Constraints (NOT NULL, UNIQUE, CHECK, DEFAULT)
•	Referential integrity with cascade deletes on dependent tables
Sample Data:
•	2 Organisers with organisation details
•	2 Participants with personal information
•	3 Events (Durban City Marathon, Durban 10km Walk, Cape Town Cycle Tour)
•	7 Categories covering both age and distance types
•	3 Sample enrolments with payment statuses
•	2 Example results with finish times and positions
Testing: The script runs without errors on a clean SQL Server instance and includes verification queries at the bottom showing all populated tables and joined views.
________________________________________
⚠️ Deviations from Original Part 1 Submission
During the review of Part 1, several design issues were identified in the original ERD that would have prevented Part 2 from meeting the brief's functional requirements. The following changes were made to ensure consistency across all parts.
Changes Made:
Entity	Change	Reason
Organisers	Removed EventId	An Organiser creates many Events; the original relationship direction was reversed
Participants	Removed EventId	Participants enrol in Categories via Enrolments; direct Event linkage was incorrect
Events	Removed CategoryId; added Distance and EventType	One Event has many Categories; Part 2 brief requires distance and event type fields
Categories	Removed UserId; added EventId and CategoryType	Categories belong to Events; Part 2 brief requires Age OR Distance category types
Enrolments	Added CategoryId	Participants must select a category when enrolling (required by Part 2 brief)
Results	Removed OrganiserId	Results link through Enrolments; direct Organiser linkage was redundant
Assignments	Removed table entirely	Not required for Part 2 functionality; simplified the schema
Technical Note on Cascade Behaviour:
FK_Enrolments_Categories uses NO ACTION (instead of CASCADE) to prevent SQL Server's "multiple cascade paths" error. This means deleting a Category with active Enrolments will fail — API logic handles this by validating before deletion.
Impact:
•	The ERD, SQL script, and endpoint plan in /docs have been updated to reflect these changes
•	All Part 1 documents are now consistent with the Part 2 implementation
•	The core entities (Users, Organisers, Participants, Events, Categories, Enrolments, Results) remain the same
________________________________________
✅ CI/CD Pipeline
Workflow: .github/workflows/validate-docs.yml
The GitHub Actions workflow automatically validates the repository structure on every push and pull request, ensuring all required documents are present.
Validation Checks:
•	✅ /docs folder exists
•	✅ ERD image (PNG or PDF) is present
•	✅ Endpoint plan (Markdown) is present
•	✅ SQL script (SQL) is present
Green Build Status:
https://docs/cicd-green-build.png
Screenshot of successful GitHub Actions build
________________________________________
🚀 Setup Instructions
Prerequisites
•	SQL Server Management Studio (SSMS)
•	Git
•	GitHub Account
•	(For Part 2) Visual Studio 2022 with .NET 8 SDK
Steps
1.	Clone the Repository
bash
git clone https://github.com/Rea-0611/RaceDay-Prog-2B-POE.git
cd RaceDay-Prog-2B-POE
2.	Review Planning Documents
o	Open docs/RaceDay_ERD.png to understand the database structure
o	Read docs/endpoint-plan.md to understand the API design
o	Examine docs/RaceDay_Database_Creation.sql for database implementation
3.	Run the Database Script
o	Open SQL Server Management Studio
o	Connect to your SQL Server instance
o	Open docs/RaceDay_Database_Creation.sql
o	Execute Step 1 (Cleanup) against master database
o	Execute Steps 2-7 sequentially
o	Verify no errors occurred
4.	Verify Database Creation
sql
USE RaceDay;

-- Check row counts
SELECT 'Users' AS TableName, COUNT(*) AS RowCount FROM Users
UNION ALL SELECT 'Organisers', COUNT(*) FROM Organisers
UNION ALL SELECT 'Participants', COUNT(*) FROM Participants
UNION ALL SELECT 'Events', COUNT(*) FROM Events
UNION ALL SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL SELECT 'Enrolments', COUNT(*) FROM Enrolments
UNION ALL SELECT 'Results', COUNT(*) FROM Results;
Expected: 4 Users, 2 Organisers, 2 Participants, 3 Events, 7 Categories, 3 Enrolments, 2 Results


RaceDay - Event Management System (Part 2)
📋 Overview
RaceDay is a comprehensive web-based event management platform designed specifically for the South African road running, walking, and cycling community. The system bridges the gap between event organisers and participants by replacing outdated paper-based registrations with a modern, integrated digital solution.

This repository contains Part 1 (System Planning & Database) and Part 2 (RESTful API Development) of the three-part PROG6212 Portfolio of Evidence.

👥 User Roles
The system supports two distinct user roles with specific permissions:

🏢 Organiser
Event organisers have full control over their events and can:

Create, edit, and delete events

Define age or distance categories for each event

Capture participant results (finish times and positions)

View all enrolments for their events

Update participant payment statuses

Manage event status (Draft, Open, Closed, Finished)

🏃 Participant
Race participants can engage with the platform through:

Account creation and profile management

Browsing upcoming events

Entering events by selecting a category

Viewing their enrolment history

Tracking personal performance results

Checking event details including date, location, distance, and type

📁 Project Structure
text
RaceDay-Prog-2B-POE/
│
├── RaceDay.API/                          ← Main API project (.NET 8)
│   ├── Controllers/                      ← 6 controllers (21 endpoints)
│   │   ├── AuthController.cs
│   │   ├── UsersController.cs
│   │   ├── EventsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── EnrolmentsController.cs
│   │   └── ResultsController.cs
│   ├── Data/
│   │   └── RaceDayDbContext.cs           ← EF Core database context
│   ├── Models/
│   │   ├── Entities/                     ← 7 database entities
│   │   └── DTOs/                         ← 14 request/response DTOs
│   ├── Services/                         ← 6 business logic services
│   ├── appsettings.json                  ← DB connection + JWT config
│   └── Program.cs                        ← Application startup
│
├── RaceDay.Tests/                        ← Unit tests (all about Jarred)
│   ├── Helpers/
│   │   └── TestHelper.cs
│   ├── AuthTests.cs
│   ├── EventTests.cs
│   ├── EnrolmentTests.cs
│   ├── ResultTests.cs
│   └── RoleAccessTests.cs
│
├── docs/                                 ← Part 1 planning documents
│   ├── RaceDay_ERD.png
│   ├── endpoint-plan.md
│   └── RaceDay_Database_Creation.sql
│
├── .github/workflows/
│   └── ci.yml                            ← GitHub Actions CI/CD
│
└── README.md
📊 Part 1 Deliverables
1. Entity Relationship Diagram (ERD)
File: docs/RaceDay_ERD.png

Visual blueprint of the database with 7 entities, primary keys, foreign keys, cardinalities, and action words.

Entity	Description
Users	Base table for authentication and profile info
Organisers	Extends Users with organisation details
Participants	Extends Users with participant details
Events	Event info (name, date, location, distance, type, status)
Categories	Age or Distance categories per event
Enrolments	Participant sign-ups for specific categories
Results	Finish times and positions linked to enrolments

2. API Endpoint Plan
File: docs/endpoint-plan.md

Table of all 21 RESTful endpoints with HTTP method, route, description, role, request body, and expected response.

Category	Endpoints
Authentication	2
Profile	2
Events	5
Categories	4
Enrolments	4
Results	4
Total	21

3. SQL Database Script
File: docs/RaceDay_Database_Creation.sql

Complete database schema for SQL Server Management Studio (SSMS) including:

DROP/CREATE TABLE statements for all 7 entities

Primary keys, foreign keys, and constraints

Sample data (2 Organisers, 2 Participants, 3 Events, 7 Categories, 3 Enrolments, 2 Results)

Verification queries showing all populated tables


🚀 Part 2 Deliverables
1. RESTful API in C# (ASP.NET Core .NET 8)
All 21 endpoints from Part 1 implemented, plus one additional endpoint for participants to view their own results.

Key Features:

Entity Framework Core with Code-First approach

JWT authentication with role-based access control

BCrypt password hashing (never stored plain)

Swagger UI for live endpoint testing

DTOs separate from entities for security and flexibility

Service layer for clean separation of concerns



2. Authentication & Role-Based Access
Feature	Implementation
User registration	POST /api/auth/register — creates User + role-specific record
Login	POST /api/auth/login — returns JWT token
Session management	JWT bearer token (60 min expiry)
Password hashing	BCrypt.Net-Next
Role enforcement	[Authorize(Roles = "Organiser")] and [Authorize(Roles = "Participant")]


3. Functional Requirements Coverage
Requirement	Status
Register/login based on role	✅
Both roles view/update own profile	✅
Organisers manage events (create/update/delete)	✅
Both roles view events	✅
Events capture name, description, date, location, distance, event type	✅
Organisers define age OR distance categories	✅
Both roles view categories	✅
Participants enrol by selecting a category	✅
Organisers view all enrolments for their events	✅
Organisers capture finish times and positions	✅
Participants view their own results	✅
Swagger integration with all endpoints visible	✅
Role-based access enforced at API level	✅


4. Unit Tests — All About Jarred
The Part 2 brief requires tests focused on "Jarred". Every test uses Jarred as the main character to verify business rules and role-based access.

Test Coverage (23 tests across 5 files):

File	Tests	Coverage
AuthTests.cs	8	Register, login, duplicate email, invalid role, password hashing
EventTests.cs	5	Create, view, update, delete events as Jarred the Organiser
EnrolmentTests.cs	4	Enrol, prevent duplicates, view own enrolments
ResultTests.cs	3	Capture results, prevent duplicates, view own results
RoleAccessTests.cs	3	Jarred cannot modify other organisers' events
Example test names:

JarredCanRegisterAsOrganiser

JarredCannotRegisterWithDuplicateEmail

JarredAsOrganiserCanCreateEvent

JarredCannotEnrolInSameCategoryTwice

JarredAsOrganiserCannotUpdateAnotherOrganisersEvent

JarredPasswordIsHashed


5. Swagger Integration
Swagger UI is enabled for testing all endpoints directly in the browser. Each endpoint has:

Clear description of its purpose

Request body schema

Expected response codes

JWT authorization button


⚠️ Deviations from Original Part 1 Submission
During Part 1 review, several design issues were identified that would have prevented the API from meeting Part 2's functional requirements. The following changes were made and documented here for transparency.

Entity	Change	Reason
Organisers	Removed EventId	An Organiser creates many Events; relationship was reversed
Participants	Removed EventId	Participants enrol in Categories via Enrolments
Events	Removed CategoryId; added Distance and EventType	One Event has many Categories; Part 2 requires distance and type fields
Categories	Removed UserId; added EventId and CategoryType	Categories belong to Events; Part 2 requires Age OR Distance categories
Enrolments	Added CategoryId	Participants must select a category when enrolling
Results	Removed OrganiserId	Results link through Enrolments
Assignments	Removed table entirely	Not required for Part 2 functionality
Technical Note
FK_Enrolments_Categories uses ON DELETE NO ACTION (instead of CASCADE) to prevent SQL Server's "multiple cascade paths" error. The API validates before deletion to handle this gracefully.

Addition: We added one endpoint not in the original Part 1 plan: GET /api/participants/me/results. This is required by the Part 2 brief so participants can view their own results.

✅ CI/CD Pipeline
Workflow: .github/workflows/ci.yml

The GitHub Actions workflow runs on every push and pull request. It:

Sets up .NET 8 SDK

Restores dependencies

Builds the solution

Runs all unit tests

Green Build Status:

https://docs/cicd-green-build.png

Screenshot of successful GitHub Actions build — all tests passing

🚀 Setup Instructions
Prerequisites
Visual Studio 2022 (v17.8 or later)

.NET 8 SDK

SQL Server Management Studio (SSMS)

Git

Step 1: Clone the Repository
bash
git clone https://github.com/Rea-0611/RaceDay-Prog-2B-POE.git
cd RaceDay-Prog-2B-POE
Step 2: Create the Database
Open SQL Server Management Studio

Open docs/RaceDay_Database_Creation.sql

Execute the script (F5)

Verify the 7 tables appear in RaceDay database

Step 3: Configure the Connection String
Open RaceDay.API/appsettings.json and verify:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RaceDay;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
Adjust the server name if you're using a full SQL Server instance.

Step 4: Build and Run the API
In Visual Studio:

Open the solution (RaceDay.sln)

Right-click RaceDay.API → Set as Startup Project

Press F5 (or Ctrl+F5)

Swagger opens at https://localhost:{port}/swagger

Or via command line:

bash
cd RaceDay.API
dotnet run
Step 5: Run the Unit Tests
In Visual Studio:

Test → Test Explorer (Ctrl + E, T)

Click Run All Tests (green ▶️)

Or via command line:

bash
dotnet test
Expected: All 23 tests pass.

🧪 Testing the API with Swagger
Quick Flow to Test the API
1. Register a user as Organiser:

json
POST /api/auth/register
{
  "email": "jarred.org@raceday.com",
  "password": "Jarred123!",
  "fullName": "Jarred the Organiser",
  "role": "Organiser",
  "organisationName": "Jarred's Running Club"
}
2. Register a user as Participant:

json
POST /api/auth/register
{
  "email": "jarred.part@raceday.com",
  "password": "Jarred123!",
  "fullName": "Jarred the Participant",
  "role": "Participant",
  "dateOfBirth": "1990-05-15",
  "contactNumber": "0821234567"
}
3. Login and get a JWT token:

json
POST /api/auth/login
{
  "email": "jarred.org@raceday.com",
  "password": "Jarred123!"
}
Copy the token from the response.

4. Authorize Swagger:

Click the green Authorize button (top-right of Swagger)

Paste the token (without "Bearer" prefix)

Click Authorize → Close

5. Create an event (as Organiser):

json
POST /api/events
{
  "eventName": "Jarred's Marathon",
  "eventDate": "2026-12-01T06:00:00",
  "location": "Durban",
  "description": "A marathon for Jarred",
  "distance": "42.2km",
  "eventType": "Run",
  "eventStatus": "Open"
}
6. Add a category (as Organiser):

json
POST /api/events/1/categories
{
  "categoryName": "Full Marathon",
  "categoryType": "Distance",
  "entryFee": 250.00
}
7. Login as Participant, authorize with the new token, then enrol:

json
POST /api/events/1/categories/1/enrol
{
  "paymentStatus": "Pending"
}
8. View participant's enrolments:

text
GET /api/participants/me/enrolments
📹 Video Presentation

Watch Part 1 Walkthrough:
https://youtu.be/qrXjeQV9ZrY

Watch Part 2 Walkthrough:


The Part 2 video covers:

Code structure and folder organisation

JWT authentication implementation

Role-based access control at API level

Live Swagger demo (register, login, create event, enrol)

Unit test run showing all 23 tests passing

GitHub Actions CI/CD pipeline

🔄 What's Next
This repository contains Part 1 and Part 2. Part 3 will add:

Part	Description
Part 3	MVC web application consuming the API, Azure Blob Storage integration, and Docker containerisation
📝 Submission Checklist
Part 1
☑ ERD created and saved as PNG/PDF in /docs
☑ Endpoint plan completed as Markdown in /docs
☑ SQL script written and tested in /docs
☑ Minimum 20 meaningful commits
☑ CI/CD workflow configured
☑ Green build achieved
Part 2
☑ ASP.NET Core Web API created (.NET 8)
☑ EF Core with Code-First approach
☑ 21 endpoints implemented (plus 1 justified addition)
☑ JWT authentication with role-based access
☑ Passwords hashed with BCrypt
☑ Swagger integration with all endpoints
☑ 23 unit tests (all about Jarred)
☑ GitHub Actions builds + runs tests
☑ README with setup instructions
☑ YouTube video link included
📞 Contact
Student: Pontsho Sedumedi
Student Number: ST10483202
Student Email: st10483202@rcconnect.edu.za
Module: Programming 2B (PROG6212)
Institution: RoseBank International Braamfontein

Version: 2.0 — Part 1 (Revised) + Part 2 (API Development)
Date: October 2026
Course: PROG6212 - Programming 2B

📚 References
DeepSeek. 2026. DeepSeek-V3. [Generative AI]. Available at: https://deepseek.com [Accessed October 2026].

draw.io. 2026. Flowchart Maker and Online Diagram Software. [Online]. Available at: https://diagrams.net [Accessed October 2026].

Microsoft. 2024. ASP.NET Core Documentation. [Online]. Available at: https://learn.microsoft.com/aspnet/core [Accessed October 2026].

🔗 Important Links
GitHub Repository: https://github.com/Rea-0611/RaceDay-Prog-2B-POE.git

Part 1 YouTube Video: https:https://youtu.be/qrXjeQV9ZrY

Part 2 YouTube Video: [Replace with your unlisted Part 2 video link]

