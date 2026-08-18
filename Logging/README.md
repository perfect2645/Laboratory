# Common Logging Library
## A versatile logging library for .NET applications.

## Serilog in Hosting (Recommended)

### Use case 1 : Logging to console / debug

Sample : [Logging to console](https://github.com/perfect2645/Laboratory/blob/main/LoggingDemo/SerilogConsole.cs)

```csharp	

SeriLogger.WriteToConsole();

var strParameter = "some string";

Log.Information("{strParameter}Information", strParameter);
Log.Warning("{strParameter}Warning", strParameter);
Log.Error("{strParameter}Error", strParameter);


```

```
// enrich your logs

SeriLogger.WriteToConsole(loggerConfiguration =>
{
    loggerConfiguration.Enrich.WithProperty("Application", "LoggingDemo");
});
Log.Information("Enriched Information");

```
## Serilog in file

Sample : [Logging to file](https://github.com/perfect2645/Laboratory/blob/main/LoggingDemo/SerilogFile.cs)

``` csharp
SeriLogger.WriteToFile(logPath: "logs/demo-.log",
    configLog: loggerConfiguration =>
{
    loggerConfiguration.Enrich.WithProperty("Application", "LoggingDemo");
});

const string method = "LogEnrichFile";
Log.Information("{LogEnrichFile} Information", method);
Log.Warning("{LogEnrichFile} Warning", method);
Log.Error("{LogEnrichFile} Error", method);

```

## Serilog in file with json configuration (Recommended)

Sample : [Serilog in file with json configuration](https://github.com/perfect2645/Laboratory/blob/main/LoggingDemo/SerilogFileWithConfig.cs)

``` csharp

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("serilog.config.json")
    .Build();

SeriLogger.ReadFromConfiguration(configuration);

const string method = "LogFromConfiguration";
Log.Information("{LogFromConfiguration} Information", method);
Log.Warning("{LogFromConfiguration} Warning", method);
Log.Error("{LogFromConfiguration} Error", method);

```

``` json
// application.json or any other json configuration

{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore.SignalR": "Debug"
      }
    },
    "Enrich": [
      "FromLogContext",
      "WithThreadId",
      "WithStackTrace"
    ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}]{Application} [ThreadId:{ThreadId}] {SourceContext} : {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}]{Application} [ThreadId:{ThreadId}] {SourceContext} : {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}

```

## Serilog in Hosting with json configuration (Recommended)

Sample : [Serilog in a hosting based WPF program](https://github.com/perfect2645/artifact-recognition/blob/main/artifact-desktop/artifact-desktop/App.xaml.cs)

```
    var hostBuilder = Host.CreateDefaultBuilder(args)
        .AddSerilogger()
```

## Log4net in Webapi (Obsolete since ver 2.0.0)

```
// specify log output path (default - app base dir / logs)
builder.Logging.NetCoreLoggingSetup(Path.Combine("logs", builder.Environment.ApplicationName));
```