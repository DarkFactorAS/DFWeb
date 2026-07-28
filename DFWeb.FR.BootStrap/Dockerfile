# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy everything else
COPY . .

# Restore dependencies
RUN --mount=type=secret,id=nuget_username,env=NUGET_USERNAME,required=true \
    --mount=type=secret,id=nuget_token,env=NUGET_TOKEN,required=true \
    dotnet restore ./DFWeb.FR.BootStrap/DFWeb.FR.BootStrap.csproj --configfile ./DFWeb.FR.BootStrap/NuGet.config

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
