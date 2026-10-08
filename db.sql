-- ============================================================================
-- CRM Lead Follow-up System - Complete Database Creation & Seed Script
-- Target DB: Microsoft SQL Server 2019+ / Azure SQL
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CRM_LeadSystem')
BEGIN
    CREATE DATABASE CRM_LeadSystem;
END
GO

USE CRM_LeadSystem;
GO

-- ============================================================================
-- 1. DROP EXISTING TABLES (In Reverse Dependency Order for Clean Resets)
-- ============================================================================
IF OBJECT_ID('dbo.AuditLogs', 'U') IS NOT NULL DROP TABLE dbo.AuditLogs;
IF OBJECT_ID('dbo.LeadEvaluations', 'U') IS NOT NULL DROP TABLE dbo.LeadEvaluations;
IF OBJECT_ID('dbo.Notes', 'U') IS NOT NULL DROP TABLE dbo.Notes;
IF OBJECT_ID('dbo.FollowUps', 'U') IS NOT NULL DROP TABLE dbo.FollowUps;
IF OBJECT_ID('dbo.Activities', 'U') IS NOT NULL DROP TABLE dbo.Activities;
IF OBJECT_ID('dbo.Leads', 'U') IS NOT NULL DROP TABLE dbo.Leads;
IF OBJECT_ID('dbo.LeadStages', 'U') IS NOT NULL DROP TABLE dbo.LeadStages;
IF OBJECT_ID('dbo.Contacts', 'U') IS NOT NULL DROP TABLE dbo.Contacts;
IF OBJECT_ID('dbo.Companies', 'U') IS NOT NULL DROP TABLE dbo.Companies;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- ============================================================================
-- 2. CREATE TABLES
-- ============================================================================

-- Table 1: Users
CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(20) NOT NULL CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'Sales Manager', 'Sales Rep')),
    ManagerId INT NOT NULL CONSTRAINT FK_ManagerId FOREIGN KEY REFERENCES  dbo.Users(Id),



    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 0,  -- By default the user is not granted access. A manager must grant access
    IsDeleted BIT NOT NULL CONSTRAINT DF_Users_IsDeleted DEFAULT 0
);

CREATE UNIQUE NONCLUSTERED INDEX UX_Users_Email ON dbo.Users(Email) WHERE IsDeleted = 0;

-- Table 2: Companies
CREATE TABLE dbo.Companies (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Companies PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Industry NVARCHAR(100) NULL,
    EmployeeCount INT NULL,
    AnnualRevenue DECIMAL(18,2) NULL,
    Website NVARCHAR(200) NULL,
    Phone NVARCHAR(30) NULL,



    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Companies_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Companies_IsActive DEFAULT 1,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Companies_IsDeleted DEFAULT 0
);
CREATE UNIQUE NONCLUSTERED INDEX UX_Companies_Name ON dbo.Companies(Name) WHERE IsDeleted = 0;

-- Table 3: Contacts  ( Each company can have multiple contacts )
CREATE TABLE dbo.Contacts (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Contacts PRIMARY KEY,
    CompanyId INT NULL CONSTRAINT FK_Contacts_Companies FOREIGN KEY REFERENCES dbo.Companies(Id),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(30) NULL,
    JobTitle NVARCHAR(100) NULL,



    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Contacts_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Contacts_IsActive DEFAULT 1,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Contacts_IsDeleted DEFAULT 0
);

CREATE UNIQUE NONCLUSTERED INDEX UX_Contacts_Email ON dbo.Contacts(Email) WHERE IsDeleted = 0;

-- Table 4: LeadStages 
CREATE TABLE dbo.LeadStages (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LeadStages PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    DisplayOrder INT NOT NULL,
    ConversionWeight DECIMAL(5,2) NOT NULL,


    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_LeadStages_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LeadStages_IsActive DEFAULT 1,
    IsDeleted BIT NOT NULL CONSTRAINT DF_LeadStages_IsDeleted DEFAULT 0
);
CREATE UNIQUE NONCLUSTERED INDEX UX_LeadStages_Name ON dbo.LeadStages(Name) WHERE IsDeleted = 0;

-- Table 5: Leads (Lead is a sales oppurtunity)
CREATE TABLE dbo.Leads (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Leads PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    ContactId INT NOT NULL CONSTRAINT FK_Leads_Contacts FOREIGN KEY REFERENCES dbo.Contacts(Id),
    CompanyId INT NULL CONSTRAINT FK_Leads_Companies FOREIGN KEY REFERENCES dbo.Companies(Id),
    LeadStageId INT NOT NULL CONSTRAINT FK_Leads_LeadStages FOREIGN KEY REFERENCES dbo.LeadStages(Id), -- What is current stage of lead
    AssignedToUserId INT NOT NULL CONSTRAINT FK_Leads_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    Source NVARCHAR(50) NOT NULL,
    EstimatedValue DECIMAL(18,2) NULL,
    CurrentScore DECIMAL(5,2) NULL,


    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Leads_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Leads_IsActive DEFAULT 1,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Leads_IsDeleted DEFAULT 0
);
CREATE NONCLUSTERED INDEX IX_Leads_AssignedStage ON dbo.Leads(AssignedToUserId, LeadStageId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Leads_Title ON dbo.Leads(Title) WHERE IsDeleted = 0;

-- Table 6: Activities
CREATE TABLE dbo.Activities (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Activities PRIMARY KEY,
    LeadId INT NOT NULL CONSTRAINT FK_Activities_Leads FOREIGN KEY REFERENCES dbo.Leads(Id),
    ActivityType NVARCHAR(30) NOT NULL CONSTRAINT CK_Activities_Type CHECK (ActivityType IN ('Call', 'Meeting', 'Email', 'Demo')),
    Subject NVARCHAR(200) NOT NULL,
    Summary NVARCHAR(MAX) NULL,
    ActivityDate DATETIME2 NOT NULL,
    LoggedByUserId INT NOT NULL CONSTRAINT FK_Activities_Users FOREIGN KEY REFERENCES dbo.Users(Id),


    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Activities_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Activities_IsActive DEFAULT 1,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Activities_IsDeleted DEFAULT 0
);
CREATE NONCLUSTERED INDEX IX_Activities_LeadDate ON dbo.Activities(LeadId, ActivityDate) WHERE IsDeleted = 0;

-- Table 7: FollowUps
CREATE TABLE dbo.FollowUps (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FollowUps PRIMARY KEY,
    LeadId INT NOT NULL CONSTRAINT FK_FollowUps_Leads FOREIGN KEY REFERENCES dbo.Leads(Id),
    Title NVARCHAR(200) NOT NULL,
    DueDate DATETIME2 NOT NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT CK_FollowUps_Status CHECK (Status IN ('Pending', 'Completed', 'Cancelled')) DEFAULT 'Pending',
    Priority NVARCHAR(20) NOT NULL CONSTRAINT CK_FollowUps_Priority CHECK (Priority IN ('Low', 'Medium', 'High')) DEFAULT 'Medium',
    AssignedToUserId INT NOT NULL CONSTRAINT FK_FollowUps_Users FOREIGN KEY REFERENCES dbo.Users(Id),


    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_FollowUps_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_FollowUps_IsActive DEFAULT 1,
    IsDeleted BIT NOT NULL CONSTRAINT DF_FollowUps_IsDeleted DEFAULT 0
);
CREATE NONCLUSTERED INDEX IX_FollowUps_AssignedStatus ON dbo.FollowUps(AssignedToUserId, Status, DueDate) WHERE IsDeleted = 0;

-- Table 8: Notes
CREATE TABLE dbo.Notes (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notes PRIMARY KEY,
    LeadId INT NOT NULL CONSTRAINT FK_Notes_Leads FOREIGN KEY REFERENCES dbo.Leads(Id),
    Content NVARCHAR(MAX) NOT NULL,


    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Notes_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SystemSeed',
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Notes_IsActive DEFAULT 1,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Notes_IsDeleted DEFAULT 0
);

-- Table 9: LeadEvaluations (AI/ML Audit Log Table)
CREATE TABLE dbo.LeadEvaluations (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LeadEvaluations PRIMARY KEY,
    LeadId INT NOT NULL CONSTRAINT FK_LeadEvaluations_Leads FOREIGN KEY REFERENCES dbo.Leads(Id),
    EvaluatedAt DATETIME2 NOT NULL CONSTRAINT DF_LeadEvaluations_EvaluatedAt DEFAULT SYSUTCDATETIME(),
    CalculatedScore DECIMAL(5,2) NOT NULL,
    ConfidenceRating NVARCHAR(20) NOT NULL CONSTRAINT CK_LeadEvaluations_Confidence CHECK (ConfidenceRating IN ('High', 'Medium', 'Low')),
    InputDataJson NVARCHAR(MAX) NOT NULL,
    ScoreBreakdownJson NVARCHAR(MAX) NOT NULL,
    IsUserOverridden BIT NOT NULL CONSTRAINT DF_LeadEvaluations_IsOverridden DEFAULT 0,
    OverriddenScore DECIMAL(5,2) NULL,
    OverrideReason NVARCHAR(500) NULL,
    EvaluatedByUserId INT NOT NULL CONSTRAINT FK_LeadEvaluations_Users FOREIGN KEY REFERENCES dbo.Users(Id)
);

-- Table 10: AuditLogs
CREATE TABLE dbo.AuditLogs (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLogs PRIMARY KEY,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId INT NOT NULL,
    Action NVARCHAR(20) NOT NULL,
    ChangesJson NVARCHAR(MAX) NULL,
    PerformedByUserId INT NOT NULL CONSTRAINT FK_AuditLogs_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    Timestamp DATETIME2 NOT NULL CONSTRAINT DF_AuditLogs_Timestamp DEFAULT SYSUTCDATETIME()
);
GO

-- ============================================================================
-- 3. SEED DATA GENERATION
-- ============================================================================

-- Seed 1: Users (Hashes represent default password "Password123!")
INSERT INTO dbo.Users (FullName, Email, PasswordHash, Role)
VALUES 
('System Admin', 'admin@crm.com', 'AQAAAAEAACcQAAAAEHx82m8X+samplehashforadmin...', 'Admin'),
('Sarah Connor', 'sarah.m@crm.com', 'AQAAAAEAACcQAAAAEHx82m8X+samplehashformgr...', 'Sales Manager'),
('John Doe', 'john.rep@crm.com', 'AQAAAAEAACcQAAAAEHx82m8X+samplehashforrep1...', 'Sales Rep'),
('Alice Smith', 'alice.rep@crm.com', 'AQAAAAEAACcQAAAAEHx82m8X+samplehashforrep2...', 'Sales Rep'),
('Robert Bruce', 'robert.rep@crm.com', 'AQAAAAEAACcQAAAAEHx82m8X+samplehashforrep3...', 'Sales Rep');

-- Seed 2: LeadStages (6 pipeline steps)
INSERT INTO dbo.LeadStages (Name, DisplayOrder, ConversionWeight)
VALUES 
('New', 1, 10.00),
('Contacted', 2, 25.00),
('Qualified', 3, 50.00),
('Proposal', 4, 75.00),
('Closed Won', 5, 100.00),
('Closed Lost', 6, 0.00);

-- Seed 3: Companies (10 Master Records)
INSERT INTO dbo.Companies (Name, Industry, EmployeeCount, AnnualRevenue, Website, Phone)
VALUES 
('Acme Corporation', 'Manufacturing', 250, 15000000.00, 'https://acme.example.com', '+1-555-0101'),
('TechPulse Systems', 'Software', 45, 3500000.00, 'https://techpulse.example.com', '+1-555-0102'),
('Global Logistics Ltd', 'Transportation', 1200, 85000000.00, 'https://globallog.example.com', '+1-555-0103'),
('Apex Financial Services', 'Banking', 500, 42000000.00, 'https://apexfin.example.com', '+1-555-0104'),
('GreenTerra Energy', 'CleanTech', 80, 6000000.00, 'https://greenterra.example.com', '+1-555-0105'),
('Nexus HealthCare', 'Medical Supplies', 310, 22000000.00, 'https://nexushealth.example.com', '+1-555-0106'),
('OmniRetail Group', 'E-commerce', 150, 11000000.00, 'https://omniretail.example.com', '+1-555-0107'),
('Starlight Media', 'Marketing', 25, 1800000.00, 'https://starlight.example.com', '+1-555-0108'),
('Vanguard Robotics', 'Automation', 110, 14000000.00, 'https://vanguardbot.example.com', '+1-555-0109'),
('Horizon BioLabs', 'Pharma', 95, 9500000.00, 'https://horizonbio.example.com', '+1-555-0110');

-- Seed 4: Contacts (10 Master Records)
INSERT INTO dbo.Contacts (CompanyId, FirstName, LastName, Email, Phone, JobTitle)
VALUES 
(1, 'Michael', 'Scott', 'mscott@acme.example.com', '+1-555-0201', 'Regional Director'),
(2, 'Eleanor', 'Vance', 'evance@techpulse.example.com', '+1-555-0202', 'CTO'),
(3, 'David', 'Miller', 'dmiller@globallog.example.com', '+1-555-0203', 'VP of Operations'),
(4, 'Sophia', 'Chen', 'schen@apexfin.example.com', '+1-555-0204', 'Chief Risk Officer'),
(5, 'Marcus', 'Aurelius', 'marcus@greenterra.example.com', '+1-555-0205', 'Sustainability Lead'),
(6, 'Clara', 'Oswald', 'coswald@nexushealth.example.com', '+1-555-0206', 'Procurement Manager'),
(7, 'James', 'Holden', 'jholden@omniretail.example.com', '+1-555-0207', 'Head of IT'),
(8, 'Naomi', 'Nagata', 'nnagata@starlight.example.com', '+1-555-0208', 'Creative Director'),
(9, 'Amos', 'Burton', 'aburton@vanguardbot.example.com', '+1-555-0209', 'Lead Engineer'),
(10, 'Chrisjen', 'Avasarala', 'cavasarala@horizonbio.example.com', '+1-555-0210', 'VP Strategic Alliances');

-- Seed 5: Leads (10 Transaction Records)
INSERT INTO dbo.Leads (Title, ContactId, CompanyId, LeadStageId, AssignedToUserId, Source, EstimatedValue, CurrentScore)
VALUES 
('Enterprise ERP License Upgrade', 1, 1, 4, 3, 'Website', 120000.00, 82.50),
('Cloud Infrastructure Migration', 2, 2, 3, 3, 'Referral', 45000.00, 68.00),
('Fleet Tracking System Integration', 3, 3, 2, 4, 'Cold Call', 250000.00, 45.00),
('Core Banking API Security Audit', 4, 4, 4, 4, 'Trade Show', 95000.00, 88.00),
('Solar Farm IoT Monitoring Software', 5, 5, 1, 5, 'Website', 35000.00, 20.00),
('Hospital Inventory Mgmt Platform', 6, 6, 3, 5, 'Inbound Email', 78000.00, 62.00),
('E-commerce Checkout Optimization', 7, 7, 5, 3, 'Referral', 55000.00, 95.00),
('Digital Ad Campaign Automation', 8, 8, 6, 4, 'Cold Call', 18000.00, 0.00),
('Robotic Assembly Line Telemetry', 9, 9, 2, 5, 'Trade Show', 140000.00, 52.00),
('Clinical Trial Data Storage Suite', 10, 10, 3, 3, 'Website', 110000.00, 74.00);

-- Seed 6: Activities (10 Transaction Records)
INSERT INTO dbo.Activities (LeadId, ActivityType, Subject, Summary, ActivityDate, LoggedByUserId)
VALUES 
(1, 'Meeting', 'Initial Discovery Call', 'Discussed current ERP pain points and legacy database migration options.', DATEADD(day, -10, SYSUTCDATETIME()), 3),
(1, 'Demo', 'Product Demonstration', 'Showcased ASP.NET Core API scalability and multi-role dashboards.', DATEADD(day, -5, SYSUTCDATETIME()), 3),
(2, 'Call', 'Technical Scope Review', 'Confirmed CTO Eleanor approves cloud architecture design.', DATEADD(day, -8, SYSUTCDATETIME()), 3),
(3, 'Email', 'Introductory Deck Sent', 'Sent PDF brochures regarding fleet tracking solution.', DATEADD(day, -12, SYSUTCDATETIME()), 4),
(4, 'Meeting', 'Security & Compliance Q&A', 'Reviewed ISO 27001 requirements with Sophia Chen.', DATEADD(day, -3, SYSUTCDATETIME()), 4),
(6, 'Call', 'Budget Alignment', 'Confirmed Nexus HealthCare has allocated $80k for Q4 projects.', DATEADD(day, -6, SYSUTCDATETIME()), 5),
(7, 'Meeting', 'Contract Final Signature', 'Finalized agreements and signed deal.', DATEADD(day, -2, SYSUTCDATETIME()), 3),
(8, 'Call', 'Cold Outreach Attempt', 'Spoke to creative team; current contract tied up until next year.', DATEADD(day, -15, SYSUTCDATETIME()), 4),
(9, 'Demo', 'Telemetry Dashboard Showcase', 'Demonstrated real-time sensor processing features.', DATEADD(day, -4, SYSUTCDATETIME()), 5),
(10, 'Email', 'Data Privacy Agreement Draft', 'Emailed HIPAA compliance documentation for review.', DATEADD(day, -1, SYSUTCDATETIME()), 3);

-- Seed 7: FollowUps (10 Transaction Records)
INSERT INTO dbo.FollowUps (LeadId, Title, DueDate, Status, Priority, AssignedToUserId)
VALUES 
(1, 'Send Revised Commercial Proposal', DATEADD(day, 2, SYSUTCDATETIME()), 'Pending', 'High', 3),
(2, 'Schedule AWS Cost Estimation Session', DATEADD(day, 4, SYSUTCDATETIME()), 'Pending', 'Medium', 3),
(3, 'Follow up on Fleet Tracking Budget', DATEADD(day, -1, SYSUTCDATETIME()), 'Pending', 'High', 4),
(4, 'Draft Final SLA Contract', DATEADD(day, 1, SYSUTCDATETIME()), 'Pending', 'High', 4),
(5, 'Send First Contact Follow-up Email', DATEADD(day, 3, SYSUTCDATETIME()), 'Pending', 'Low', 5),
(6, 'Send Technical Specification PDF', DATEADD(day, 5, SYSUTCDATETIME()), 'Pending', 'Medium', 5),
(7, 'Initiate Client Onboarding Call', DATEADD(day, -3, SYSUTCDATETIME()), 'Completed', 'High', 3),
(8, 'Archive Lead File', DATEADD(day, -10, SYSUTCDATETIME()), 'Completed', 'Low', 4),
(9, 'Schedule Technical Deep Dive with Amos', DATEADD(day, 6, SYSUTCDATETIME()), 'Pending', 'Medium', 5),
(10, 'Review Data Security Feedback', DATEADD(day, 2, SYSUTCDATETIME()), 'Pending', 'High', 3);

-- Seed 8: Notes (5 Transaction Records - Totaling 35+ transaction rows across tables)
INSERT INTO dbo.Notes (LeadId, Content, CreatedBy)
VALUES 
(1, 'Client prefers communication via email before 11 AM.', 'John Doe'),
(3, 'Decision maker requires CFO approval for purchases over $200k.', 'Alice Smith'),
(4, 'High interest in automated audit trail feature.', 'Alice Smith'),
(7, 'Closed deal ahead of schedule! Onboarding starts next week.', 'John Doe'),
(10, 'Legal team requested custom indemnity clause in contract.', 'John Doe');

-- Seed 9: LeadEvaluations (Seed AI Scoring Runs for AI Audit Requirements)
INSERT INTO dbo.LeadEvaluations (LeadId, CalculatedScore, ConfidenceRating, InputDataJson, ScoreBreakdownJson, IsUserOverridden, OverriddenScore, OverrideReason, EvaluatedByUserId)
VALUES 
(1, 82.50, 'High', '{"activityCount": 2, "companySize": 250, "stage": "Proposal"}', '{"factors": ["Stage Weight (+75.00)", "2 Activities (+10.00)", "Company Size >200 (+7.50)"]}', 0, NULL, NULL, 3),
(4, 88.00, 'High', '{"activityCount": 1, "companySize": 500, "stage": "Proposal"}', '{"factors": ["Stage Weight (+75.00)", "Company Size >200 (+10.00)", "Activity Logged (+3.00)"]}', 1, 95.00, 'Manager confirmed budget approval off-line', 2);
GO

-- ============================================================================
-- VERIFICATION QUERY
-- ============================================================================
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM dbo.Users
UNION ALL SELECT 'Companies', COUNT(*) FROM dbo.Companies
UNION ALL SELECT 'Contacts', COUNT(*) FROM dbo.Contacts
UNION ALL SELECT 'LeadStages', COUNT(*) FROM dbo.LeadStages
UNION ALL SELECT 'Leads', COUNT(*) FROM dbo.Leads
UNION ALL SELECT 'Activities', COUNT(*) FROM dbo.Activities
UNION ALL SELECT 'FollowUps', COUNT(*) FROM dbo.FollowUps
UNION ALL SELECT 'Notes', COUNT(*) FROM dbo.Notes
UNION ALL SELECT 'LeadEvaluations', COUNT(*) FROM dbo.LeadEvaluations;
GO