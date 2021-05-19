# DevOps - Group H - Team Neutrals
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=albertbethlowsky_DevOpsGroupH&metric=alert_status)](https://sonarcloud.io/dashboard?id=albertbethlowsky_DevOpsGroupH)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=albertbethlowsky_DevOpsGroupH&metric=coverage)](https://sonarcloud.io/dashboard?id=albertbethlowsky_DevOpsGroupH)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=albertbethlowsky_DevOpsGroupH&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=albertbethlowsky_DevOpsGroupH)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=albertbethlowsky_DevOpsGroupH&metric=bugs)](https://sonarcloud.io/dashboard?id=albertbethlowsky_DevOpsGroupH)
[![BCH compliance](https://bettercodehub.com/edge/badge/albertbethlowsky/DevOpsGroupH?branch=main)](https://bettercodehub.com/)
[![Maintainability](https://api.codeclimate.com/v1/badges/56d1f5c74afcb819805c/maintainability)](https://codeclimate.com/github/albertbethlowsky/DevOpsGroupH/maintainability)

> This project revolves around a forum application called minitwit. The functionalities includes signing up, logging in, posting messages, following other users. The forum has a public timeline where all messages are displayed. Furthermore, if a user is signed in, a personal timeline exists that displays a users own messages aswell as messages of followed users.

## How to run the application

### Development Environement (utilizing local database in /temp)

To run the application locally, run the following command in the mvc-minitwit repository: `dotnet watch run -p mvc-minitwit.csproj`

Server running on port http://localhost:5000

To run tests of the application locally, run the following command in the HomeControllerTests repository: `dotnet test`

### Production Environment (utilizing sql server on azure)

The `azure-pipelines.yml` runs the tests, builds the dockerfile and pushes it to dockerhub, where azure listens.
The application is hosted on the following web-service domain: https://neutrals-minitwit.azurewebsites.net/

The docker-compose file in `mvc-minitwit` contains our monitoring and logging applications, which is being hosted on another webservice and are therefore not part of the azure-pipeline. When the `docker-compose.yml` is being build, it is pushed to dockerhub, where azure is listening.
The monitoring and logging applications are hosted on the following web-service domain: https://minitwit-neutrals.azurewebsites.net/

### Libraries

https://github.com/albertbethlowsky/DevOpsGroupH/network/dependencies

### Cloud dependencies

These services are responsible for cloud hosting.
| Name | Service | Provider | Description |
|------|---------|----------|-------------|
| neutrals-minitwit | App Service | Microsoft Azure | Hosting of web applications (.NET application) |
| minitwit-neutrals | App Service | Microsoft Azure | Hosting of web applications (prometheus, grafana) |
| neutralsseq       | App Service | Microsoft Azure | Hosting of web applications (Datalust - Seq) |
| minitwit-neutrals | SQL Server | Microsoft Azure | Hosting of SQL database |
| minitwitDb (minitwit-neutrals) | SQL database | Microsoft Azure | SQL database |

## Authors

Frederik Peter Volkers (frvo), Albert Bethlowsky Rovsing (arov), Rasmus Andreas de Neergaard (rade), Alekxander Austin Arkimedes Baxwill (abax), Jacob Sjöblom (jsjo).

## Report
