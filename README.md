# DevOps - Group H - Team Neutrals

[![Build Status](https://dev.azure.com/GroupNeutrals/MiniTwit/_apis/build/status/albertbethlowsky.DevOpsGroupH?branchName=main)](https://dev.azure.com/GroupNeutrals/MiniTwit/_apis/build/status/albertbethlowsky.DevOpsGroupH?branchName=main)

[![Build Status](https://dev.azure.com/GroupNeutrals/MiniTwit/_apis/build/status/albertbethlowsky.DevOpsGroupH?branchName=main)](https://dev.azure.com/GroupNeutrals/MiniTwit/_build/latest?definitionId=3&branchName=main)

> This project revolves around a forum application called minitwit. The functionalities includes signing up, logging in, posting messages, following other users. The forum has a public timeline where all messages are displayed. Furthermore, if a user is signed in, a personal timeline exists that displays a users own messages aswell as messages of followed users.

## How to run the application

### Development Environement (utilizing local database in /temp)

To run the application locally, run the following command in the mvc-minitwit repository: `dotnet watch run -p mvc-minitwit.csproj`

Server running on port http://localhost:5000

To run tests of the application locally, run the following command in the HomeControllerTests repository: `dotnet test`

### Production Environment (utilizing sql server on azure)

The `azure-pipelines.yml` runs the tests, builds the dockerfile and pushes it to dockerhub, where azure listens.
The application is hosted on the following web-service domain: http://neutrals-minitwit.azurewebsites.net/

The docker-compose file in `mvc-minitwit` contains our monitoring and logging applications, which is being hosted on another webservice and are therefore not part of the azure-pipeline. When the `docker-compose.yml` is being build, it is pushed to dockerhub, where azure is listening.
The monitoring and logging applications are hosted on the following web-service domain: https://minitwit-neutrals.azurewebsites.net/

## Dependencies

### Libraries

https://github.com/albertbethlowsky/DevOpsGroupH/network/dependencies

### Cloud dependencies

These services are responsible for cloud hosting.
| Name | Service | Provider | Description |
|------|---------|----------|-------------|
| neutrals-minitwit | App Service | Microsoft Azure | Hosting of web applications (.NET application) |
| minitwit-neutrals | App Service | Microsoft Azure | Hosting of web applications (prometheus, grafana, loki, promtail etc.) |
| minitwit-neutrals | SQL Server | Microsoft Azure | Hosting of SQL database |
| minitwitDb (minitwit-neutrals) | SQL database | Microsoft Azure | SQL database |
| jokeren9/neutralsminitwit | Docker container | DockerHub | Containerizing of applications |

## API

### Documentation

?? ADD NOTES
Postman is used for API documentation (or swagger????). Documentation

### Monitoring tools

Grafana: https://minitwit-neutrals.azurewebsites.net
Requires credentials.
Utilizes prometheus data point, which listens to `/metrics`

### Logging tools

SeriLog

## Static Tools

SonarCloud.

azsk - Secure DevOps Kit for Azure

## Authors

Frederik Peter Volkers (frvo), Albert Bethlowsky Rovsing (arov), Rasmus Andreas de Neergaard (rade), Alekxander Austin Arkimedes Baxwill (abax), Jacob Sjöblom (jsjo).

## Report

### 1) Adding Version Control

How do we collaborate?
“Long-Running Branches/Branch by Release“
Branch strategy:
Branches:
Main
Dev
Fea#<NUMBER>-DescriptionWord
Iss#<NUMBER>-DescriptionWord
Commit strategy:
Commit as soon as possible.
Commit messages:
<FILENAME> - <COMMENT> - <Potential sub feature?>
e.g. README - added titles
Pull requests:
One manages the pull request at a time.
Code review:
Review the code in collaboration.
Code review occurs at the beginning wednesday meeting.
Scrum board:
All features must be inserted to Scrum Board.
Only work on assigned features.
Add features to Scrum Board along the way.

### 2) Try to develop a high-level understanding of ITU-MiniTwit.

### 3) Migrate ITU-MiniTwit to run on a modern computer running Linux

### 4) Share your Work on Github

// ADD TO README? or update it as we go..

### 5) Refactor ITU-MiniTwit to another language and technology of your choice.

Remember to log and provide good arguments for the choice of programming language and framework. Likely, a feature mapping/comparison or a mini-benchmark is a good choice.

Why MVC is smart:
https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-5.0&tabs=visual-studio

### 6) Start implementing an API for the simulator in your ITU-MiniTwit.

your applications have to: - provide the same end points (on different hosts of course and potentially different ports) - ingest the same HTTP requests, i.e., GETs and POSTs as specified with the same parameters and payloads as provided - provide at least the same HTTP status codes in response as specified.

### 6) Introduce a DB abstraction layer in your ITU-MiniTwit

This week among other things, we were asked to “Introduce a DB abstraction layer in your ITU-MiniTwit”. We decided to use Microsoft’s Entity Framework Core as the ORM. Other common options are NHibernate or Linq2Db. The reasons for choosing EF Core over the other solutions, is due to its popularity and applicability with .NET web applications. We are also interested in gaining experiences with a solution that could benefit our career possibilities. Furthermore, EF Core is developed and supported directly by Microsoft, and therefore, it’s mature and it’s likely that it will be stable and updated looking forward.

In regards to NHibermate, EF core is quite similar, but we experienced that EF Core has more documentation available. Lastly, Linq2Db is a light-weight ORM with great performance as one of its main strengths, however as of now, performance doesn’t seem to be the main requirement of our system.

We decided to keep SQLite as the DBMS, because of convenience and popularity. SQLite is compatible with EF core and meets all our current requirements. In the following weeks we will be discussing alternative solutions such as the various noSQL systems.

### 7) Log dependencies of your ITU-MiniTwit

From now on, keep track of your dependencies. That is, all technologies, services, runtime and build-time dependencies should be logged in a corresponding file and/or visualization.

### 8) Describe Distributed Workflow

    - Which repository setup will we use?
    - Which branching model will we use?
    - Which distributed development workflow will we use?
    - How do we expect contributions to look like?
    - Who is responsible for integrating/reviewing contributions?

### 9) Consider how much you as a group adhere to the "Three Ways" characterizing DevOps (from "The DevOps Handbook"):

    - Flow
    - Feedback
    - Continual Learning and Experimentation

    Map what you are doing with regards to each principle. In case you realize you are not doing something for a principal change the way you are working as a group accordingly.

### 10) Add Monitoring to Your Systems:

### 11) Mapping Subsystems:

    - Map the all subsystems (components) of your ITU-MiniTwit in a UML component diagram.
    - That is, map all those parts that are either deployable separately or that contain bigger logical groups of functionality.
    - If rusty on component diagrams, you may want to check one of the following websites:
    - https://developer.ibm.com/articles/the-component-diagram/
    - http://www.agilemodeling.com/artifacts/componentDiagram.htm.
    - https://en.wikipedia.org/wiki/Component_diagram
