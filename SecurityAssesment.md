# Security Assesment

## Risk identification
### Assets
**The vulnerable assets within the Minitwit project.** 

 - Minitwit Web Application 
 - Minitwit API
 - Azure ressources  
	 - SQL Server and Db
	 - App service
	 - ACR
 - GitHub
	 - Codebase

### Heading
**Threat sources**

 - SQL injection attack on the web application and/or API.
 - Brute force hack on the web application and/or API.
 - XSS attack on the web application and/or API.
 - DoS attack on the web application and/or API.
 - Hack GitHub user account.
 - Hack Azure account.
 - Insecure pacages or libraries.

### Risk senarios


**SQL injection**: An attacker injects SQL into a input field on the web application. Thereby gaining access to confidential user information in the database, or modify/delete data.  
 - **Brute force attack**: An attacker brute forces their way into a users account. Thereby gaining access to confidential user information and pose as the user and post tweets. 
 - **XSS:** An attacker injects JS into an input field. Thereby hijacking a session cookie and gain access to a users account.   
 - **DoS:** An attacker creates a script that targets the */register* endpoint of the API, and registers a billion users. Thereby disupting the service of the system or redering it unavaliable. 
 - **Hack GitHub account**: An attacker hacks a teammembers GiyHub account and modifies/deletes the codebase. Thereby rendering the application unavaliable or gaining access to confidential information.
 - **Hack Azure account:** An attacker hacks a teammembers Azure account and gains access to the azure ressources associated with the Minitwit project. Thereby gaining access to confidential information or modyfing/deleting critical system ressources. 
 - **Insecure pacages or libraries**: A system pacage/dependency contains an intetional or unintentional security vulnerability. Thereby compromising the integrity of the system. 
## Risk analysis

### Likelyhood & impact
