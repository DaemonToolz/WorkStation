# WorkStation
Workstation is a simplified and open collaboritve tools.


## What is it?
By itself, it is a bundle divided into 2 projects, a client-side and a server-side.
The client project regroups all targets:
  - Browser, using ASP.NET MVC, framework 4.6.2, and SignalR : Probable evolution towards an ASP.NET Core 2.0 MVC + Angular 5 & SignalR 
  - Application, using WPF and framework 4.6.2
  - Mobile, hopefully on Android, iOS and Windows.
  
  - One part will be both on Client and Serverside and mostly written in Go for large data transfert and treatment across the network. 
  
The server project:
  - Authentication Services, token-based for the most. This one intends to be used on a wider range of projects not, necessarily, combining a username / password + token auth. Using ASP.NET Core 1.
  - Global Management Services, for a direct control of the database. It is based on a Windows Authentication system, restricting the utilization. Using ASP.NET MVC and framework 4.6.2.
  - Database Management Services, local Windows Services to clean and review the databases.
  - Workstation Services, the "mainframe" WCF service interacting with the previous clients. This one is based both on a Username / Password verification and a Token generation. It could have been simpler, yet this project is mainly an "experimentation". 
  
Both projects are / will be checked with SonarQube.
 
## How does it work?

## Note
Right now, the entire structure is based on a Monolithic architecture - everything's managed by 1 service - yet if the amount of actions increases, then Microservices will be considered.
