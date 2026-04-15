Project is a document logging system  - “CLERC SYSTEM” -  built on Asp.Net 10 built with aim to follow high maintainability, scalability and security. 

Technology stack
•	Backend: .NET SDK (.net 10), C# Visual Studio 2026
•	FrontEnd: Razor Pages, Bootsrap 5, JQUERY/AJAX 
•	Data : SQL Server / LocalDB – Entity Framework Core
•	Design patterns: Rpository pattern, Services Pattern, ViewModel pattern

GITHUB HTTPS:  https://github.com/vdjalov/Project-Defence.git
GITHUB CLI: gh repo clone vdjalov/Project-Defence
VIDEO LINK:

PROJECT ARCHITECTURE
•	DATA LAYER
 ClercSystem.Data: Contains the DbContext and data access configuration. DB Migrations and application initial seeder logic for users and entities.
 ClercSystem.Data.Models: Contains core domain entities and database schema definitions.
 ClercSystem.Infrastructure: Implementation of repository logic, implemented Repository     Pattern.

•	BUSINESS LOGIC
ClercSystem.Services: Houses the Business Logic Layer (BLL), processing data before it reaches the Razor Pages (UI).
ClercSystem.ViewModels: Contains transfer objects (DTOs) used to pass data safely to Views.
ClercSystem.Common: Utility classes, constants, and shared helpers used across all projects.

•	WEB LAYER 
ClercSystem (Main Web App): The entry point – run project 

1.	Areas/Admin -  Secured administrative module for document log monitoring for the moment and user management, .
2.	Wwwroot - Static assets (Custom js/Logs.js for AJAX operations).
3.	Authorisation -  Custom logic for Role-Based Access Control (RBAC) policies implementation service logic implemented for flexibility.
4.	Extensions – contains configuration extensions for system. Helps with better program.cs readability. 
5.	Controllers – middleman, between data layer and user view. 

