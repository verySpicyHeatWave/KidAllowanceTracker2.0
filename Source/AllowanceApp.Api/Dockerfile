FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AllowanceApp.sln ./
COPY Source/AllowanceApp.Api/AllowanceApp.Api.csproj Source/AllowanceApp.Api/
COPY Source/AllowanceApp.Core/AllowanceApp.Core.csproj Source/AllowanceApp.Core/
COPY Source/AllowanceApp.Data/AllowanceApp.Data.csproj Source/AllowanceApp.Data/
COPY Source/AllowanceApp.Shared/AllowanceApp.Shared.csproj Source/AllowanceApp.Shared/

RUN dotnet restore Source/AllowanceApp.Api/AllowanceApp.Api.csproj

COPY . .

RUN dotnet publish Source/AllowanceApp.Api/AllowanceApp.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY Source/AllowanceApp.Api/appsettings.Release.json ./appsettings.Release.json
EXPOSE 8080
ENTRYPOINT ["dotnet", "AllowanceApp.Api.dll"]

