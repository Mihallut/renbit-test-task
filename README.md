Test Task for the .Net Trainee Camp

1. Register an account on Azure portal (https://portal.azure.com/).
2. Create Azure Blob storage account.
3. Create Azure Web App for .Net
4. Create ASP.NET WEB application and deploy it to Azure Web App.
5. UI for web app can be one of: Blazor, Angular or React.
6. On the web app on start must be only one page with Form where user can upload a file (must be added validation for only .docx files) and add the user email (validation for email).
7. Web Application is putting that file to the BLOB storage.
8. Create Azure Function with BLOB storage trigger from already created BLOB and when file is added to blob the email is sent to the user with notification the file is successfully uploaded. The URL to the file must be secured with SAS token on the BLOD storage. SAS token must be valid only for 1 hour.
9. Unit tests for backend logic of web app and azure function must be added.

As a result, you must provide a link to Azure Web Application with the Form where user can upload the file and receive notification.

Please provide URL to the web app and the github link to the source code of web application and azure function.



Unit Test Result:
![image](https://github.com/Mihallut/renbit-test-task/assets/98767826/2d49a3c8-a90c-45d0-8fb5-e334519ab254)

Link to azure-hosted website:
https://renbit-test-task-web-app.azurewebsites.net/

Link to GitHub repository with code:
https://github.com/Mihallut/renbit-test-task

