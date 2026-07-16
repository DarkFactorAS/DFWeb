# Use .Net Core 3.1 image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

ARG username
ARG token
ENV NUGET_USERNAME $username
ENV NUGET_TOKEN $token

# Copy everything else
COPY . .

# Restore dependencies
RUN dotnet restore ./DFWeb.FR.BootStrap/DFWeb.FR.BootStrap.csproj --configfile ./DFWeb.FR.BootStrap/NuGet.config

# Build solution
RUN dotnet publish ./DFWeb.FR.BootStrap/DFWeb.FR.BootStrap.csproj -c Release -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "DFWeb.FR.BootStrap.dll"]

EXPOSE 5000

#docker build -t dotnetcoredarkfactorweb .
#docker run -d -p 8080:80 --name dotnetcoredarkfactorweb dotnetcoredarkfactorweb
