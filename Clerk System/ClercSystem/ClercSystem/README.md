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

When starting the project Departments, Categories, roles, and the basic admin user tables should be populated automatically via the ClercSystem.Data.Seeder folder classes, if dab already connected. We are working in development environment. 

First Credentials for login: 
admin@emaple.com	
Admin@123

  
I am using built in template(bootstrap). Built in Admin user is not a “GOD ADMIN”, and cannot change anything preset in his settings but can apply settings for other users – IsManager, Department_ID etc… You can press on the buttons but different denial pop up messages will appear showing the exact problem. Eample: 

 
 New Users are being added via the registration page. 
 
Once registered the user gets a basic observer status where he can have only view access to everything in the system. Additional fields during registration are for better clarification of the who the user is. 

   
The observer user does not have access to admin panel, cant change amend or log documents. 
 
This is the User Management Panel accessible only for the managers. We have just added the new user that is currently simply an observer. 
 
Lets add some documents using the admin Manege Document Interface. 
 

Click on add document and follow the form. You should get something like that at the end when document is added. 
 
Users Admin and User roles can edit, delete or check more details about the documents they have created. Example edit:

 
 
Notice that even the two Edit document views may look alike they are not. The second photo has a permissions dropdown that I will explain later. Via that view every user can edit the document that they own. If they try to edit someone elses document it will not work except if you have an Admin role. Still The admin user cannot delete other peoples documents, but only his own. You see the pop up message on the picture below.

  
Now lets see what happens when an ordinary user tries to delete his document. You see a document deleted successfully message and the document disappears from screen. 

 
Lets change users and log in with our Admin user. Go to Manage Documents  dropdown menu. Recent photo shows that the document the was deleted was not actually permanently deleted, but instead was only SoftDeleted or hidden and a user with an Admin Role can see that. You can see that it says undelete at the bottom, which means that it can be restored. Lets restore it. 
  
Second phot shows the restored document. 
 
If we log with the ordinary user that deleted the document in the first place. We can see that the document has been restored. And the user can again edit and delete it or can he? User with an Admin role has 2 more options in order to prevent future wrong deletions or edits.

 If we login with the Admin role user again go to manage documents and press to edit the ordinary user document we can see at the bottom permission. 
 
If we choose Read permission that means that the ordinary user will no longer be able to edit or delete its own document. If we choose Write this will allow only access to edit document without being able to delete it anymore. 

 
 
This is only possible if the Admin role user is also a manager. This is an extra filtration added to the User management profile. User admin@example.com that was seeded at the start is a manager by default. 

Now lets create another manager,  demote the  admin@example.com user to not manager and try to restrict the ordinary user again. We create secondmanager@gmail.com and promote him to an Admin Role user. So that it has more rights. That can be done only via the original user  admin@example.com user management panel. 
 
Now we log in with the secondmanager@gmail.com profile and remove the IsManager role from the original admin@example.com user. We first need to deassign the admin role and the can deassign the IsManager role. Then assign a new role to the user again. In this case we will assign admin role again. You can see on the photo below that admin@example.com – IsManager no longer. 

  

Now lets log with the original admin@example.com user and see if we can change the permissions on the document again. 

 

When we click on the edit document view we can see that this option is not available anymore. 

Admin users only can add departments via the department view and categories via the categories view. 
Finally lets check logs for documents via the Admin dropdown Check Logs. 

 

This is a view available only for the Admin role users used to check on log for every document. Every time a document is amended a log is created. When pressed on More  .js function dynamically grabs the content and show it to the user in a descending order by time logged. This is achieved using a modal view and and a partial view. Page has filtration and no pagination on purpose. 

 
At the end I will show you MYDocuemnts view where a user can focus only on his own documents without having to access other peoples documents. User can Edit, see More details and Delete. 

 

