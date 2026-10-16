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

