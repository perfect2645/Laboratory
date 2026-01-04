# .net10 webapi utils

## packages used

### C# Utils nuget package
[net.Utils.perfect2645](https://github.com/perfect2645/Laboratory/tree/main/Utils)

### Api Versioning
[ApiVersioning](https://github.com/dotnet/aspnet-api-versioning)

### Swashbuckle.AspNetCore.SwaggerGen

## Internal functions

### NetCore configurations
- IOC / DI configuration
- Api Versioning configuration
- Swagger configuration
- CORS configuration

### Filters

#### GlobalExceptionFilter

The `GlobalExceptionFilter` is an ASP.NET Core filter that handles exceptions globally across the application. 
It captures unhandled exceptions thrown during the execution of controller actions and processes them in a centralized manner.

