# Local Development

## Prerequisites

- Docker
- .NET 8.0
- Node lts

## Running the App

In the src directory go to TodaApp.client and run:

```
npm install
npm run dev
```

If you want to use only docker compose, you can run the following command in the root directory:

```
docker-compose up
```

go to the api server program and make a 

```
dotnet run 
```

else just run AppHost with Aspire as the startup project.


# Running the tests

You can execute the unit tests by running the following commands in the src directory:

1. Build the project.
```
dotnet build TodaApp.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary
```

2. Execute unit tests.
```
dotnet test TodaApp.sln
```


# Making migrations

## .NET Core CLI

With entity framework core, you can make migrations by running the following command in the src/TodaApp.Application directory.
```
dotnet ef migrations add <MIGRATION_NAME> --context DbContext
```

To generate the migration SQL script we must go to the root folder of the project and execute the following command with these arguments:
- \<PREVIOUS_MIGRATION_FILE_NAME> The file name of the previous migration.
- \<CURRENT_MIGRATION_FILE_NAME> The file name of the previous migration.
- \<SQL_OUTPUT_FILE_NAME> YYYYMMDD_filename.sql
```
dotnet ef migrations script --project src\TodaApp.Application\TodaApp.Application.csproj --startup-project src\TodaApp.Application\TodaApp.Application.csproj --context TodaApp.Application.Infrastructure.Persistence.CeraniumDbContext --configuration Debug --verbose <PREVIOUS_MIGRATION_FILE_NAME> <CURRENT_MIGRATION_FILE_NAME> --output infra/database/migrations/<SQL_OUTPUT_FILE_NAME>
```