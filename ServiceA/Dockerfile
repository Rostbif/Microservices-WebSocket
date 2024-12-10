# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the csproj files and restore as distinct layers
COPY ServiceA/ServiceA.csproj ./ServiceA/
COPY SharedLibrary/SharedLibrary.csproj ./SharedLibrary/
RUN dotnet restore ./ServiceA/ServiceA.csproj

# Copy the remaining files and build the application
COPY ServiceA/. ./ServiceA/
COPY SharedLibrary/. ./SharedLibrary/
WORKDIR /app/ServiceA
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/ServiceA/out .

# Expose the port the application runs on
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ServiceA.dll"]