FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="${PATH}:/root/.dotnet/tools"
COPY ["libman.json", "./"]
RUN libman restore

COPY ["Shared/Destuff.Shared.csproj", "Shared/"]
RUN dotnet restore "Shared/Destuff.Shared.csproj"
COPY ["Client/Destuff.Client.csproj", "Client/"]
RUN dotnet restore "Client/Destuff.Client.csproj"
COPY ["Server/Destuff.Server.csproj", "Server/"]
RUN dotnet restore "Server/Destuff.Server.csproj"

COPY . .
RUN dotnet build "Server/Destuff.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server/Destuff.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Destuff.Server.dll"]