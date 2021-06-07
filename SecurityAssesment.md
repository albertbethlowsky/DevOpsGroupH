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
 - **Insecure packages or libraries**: A system pacage/dependency contains an intetional or unintentional security vulnerability. Thereby compromising the integrity of the system. 
## Risk analysis

### Likelihood & impact

| No. | Risk                          | Risk likelihood | Risk impact |
|-----|-------------------------------|-----------------|-------------|
| 1   | SQL injection                 | low             | high        |
| 2   | Brute force attack            | medium          | low         |
| 3   | Xss attack                    | low             | low         |
| 4   | DoS attack                    | high            | medium      |
| 5   | Hack Github                   | low             | extreme     |
| 6   | Hack Azure Account            | low             | extreme     |
| 7   | Insecure packages & libraries | medium          | high        |


![img](https://i.imgur.com/I8qOcfG.png)

The white numbers are our risks mapped into our matrix.

### Discussion

**SQL injection**: By using the Entify Framework core with LINQ queries we can prevent SQL injections. 
 - **Brute force attack**: Set rules for strong passwords. By demanding a strong password, it will be more difficult for the attacker to brute force their way in. Lock out the account after a defined number of incorrect password attempts. 
 - **XSS:** By setting security headers in our project we can prevent XSS on our system. 
 - **DoS:** Prevent too many hits from any IP and be able to whitelist/blacklist these. Making use of authentication and authorization with the help of API keys. Set quotas on max connections to server and monitor abnormalities. Be able to scale the application. 
 - **Hack GitHub account**: Setup strong password for the administrator account and use two factor authentication. 
 - **Hack Azure account:** Same as protecting GitHub account. Is already protected with two factor authentication through ITU.
 - **Insecure packages or libraries**: Checking that there are no known security vulnerabilities with packages and libraries before using them. Use scanning tools to check open source packages.

