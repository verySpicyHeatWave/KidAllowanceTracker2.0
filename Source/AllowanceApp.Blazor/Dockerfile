FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AllowanceApp.sln ./
COPY Source/AllowanceApp.Blazor/AllowanceApp.Blazor.csproj Source/AllowanceApp.Blazor/
COPY Source/AllowanceApp.Shared/AllowanceApp.Shared.csproj Source/AllowanceApp.Shared/
COPY Source/AllowanceApp.Core/AllowanceApp.Core.csproj Source/AllowanceApp.Core/

RUN dotnet restore Source/AllowanceApp.Blazor/AllowanceApp.Blazor.csproj

COPY . .

RUN dotnet publish Source/AllowanceApp.Blazor/AllowanceApp.Blazor.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000

ENTRYPOINT ["dotnet", "AllowanceApp.Blazor.dll"]

