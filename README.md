# cloud-sale

This is solution for making software purchases.

# Overview
This is a web application built using .NET 8 that integrates with Azure services such as Azure App Services/Azure Container Apps and SQL Database. It allows users to manage software licenses, track purchases, and update them. The application is designed to be scalable, fault-tolerant, and easy to manage in a cloud environment.

# Environment Configuration
It is needed to define env variables on you azure resource as following:
Azure__ConnectionString
Azure__CcpGetApiUrl
Azure__CcpOrderApiUrl

so that app can read configuration 

# Running Locally
First thing first, you should Create loacl DB or use existing one and put connection string in **appsettings.json** 
Next step is to run EF Migration to create DB structure.
After that you can run app using IIS Express.
Initial data will be added to DB.
