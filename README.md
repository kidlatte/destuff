# destuff

## Getting started
* install dotnet
* restore nuget packages
* install libman
* restore libman packages

## Restore
dotnet restore
libman restore

## Build
dotnet build

## Develop
dotnet watch --project Server

## Migrate
dotnet tool install --global dotnet-ef
dotnet ef migrations --project Server add {MigrationName}

## Run
dotnet run --project Server

## Test
dotnet test Tests

### Bug: dotnet watch
set environment to development
`export ASPNETCORE_ENVIRONMENT=Development`

