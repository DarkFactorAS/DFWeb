#
# Readme
#

# This file shows example on how to set up localhost natively and in docker container

# Create two files
* appsettings.Development.json
* appsettings.DevelopmentDocker.json

#
# appsettings.Development.json
#

{
    "Logging": {
        "LogLevel": {
            "Default": "Warning",
            "Microsoft.AspNetCore": "Warning",
            "Microsoft.Hosting.Lifetime": "Warning",
            "Microsoft.EntityFrameworkCore": "Warning"
        }
    },
    "AppName": "BotServer",
    "DatabaseConnection": {
        "Server": "localhost",
        "Port": "5306",
        "Database": "",
        "Username": "",
        "Password": "",
        "SSLMode" : "Preferred"        
    },
    "EnableLogging": true,
    "reloadOnChange": false,
    "AccountServer": {
        "Endpoint": "http://localhost:5100",
        "ClientId": "",
        "ClientSecret": "",
        "Scope" : "read write"
    }
}

#
# appsettings.DevelopmentDocker.json
#

{
    "AppName": "BotServer",
    "DatabaseConnection": {
        "Server": "host.docker.internal",
        "Port": "5306",
        "Database": "",
        "Username": "",
        "Password": "",
        "SSLMode" : "Preferred"        
    },
    "EnableLogging": true,
    "reloadOnChange": false,
    "AccountServer": {
        "Endpoint": "http://host.docker.internal:5100",
        "ClientId": "",
        "ClientSecret": "",
        "Scope" : "read write"
    }
}
