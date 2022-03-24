
# WsjtxUtils.Searchlight
A .NET 6 console application that highlights callsigns within [WSJT-X](https://physics.princeton.edu/pulsar/k1jt/wsjtx.html) based on reception reports from [PSK Reporter](https://pskreporter.info/).

This application will download reception reports from PSK Reporter for the callsigns of connected WSJT-X clients. These reception reports are then correlated with recent decodes and highlighted within WSJT-X. The background color of the highlighted callsign is mapped to the range of reception report SNR values and used to indicate the relative strength of the received signal.

> **NOTE:** This software is currently in a pre-release alpha state.

## Requirements
- [.NET 6 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/)
    - Installation instruction for [Windows](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net60), [Mac](https://docs.microsoft.com/en-us/dotnet/core/install/macos), [Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux)

## Quickstart
```sh
git clone https://github.com/KC3PIB/WsjtxUtils.Searchlight

cd WsjtxUtils.Searchlight

dotnet publish -c release .\WsjtxUtils.Searchlight.sln

cd src\WsjtxUtils.Searchlight.Console\bin\Release\net6.0\publish\

WsjtxUtils.Searchlight.Console.exe
 ```
## Configuration
Options can be configured via the [config.json](https://github.com/KC3PIB/WsjtxUtils.Searchlight/blob/development/src/WsjtxUtils.Searchlight.Console/config.json) file or command-line parameters. These options include WSJT-X server IP address and port, the colors used for highlighted callsigns, reception report window and request period, and logging options.

> **NOTE:** To use other 3rd party software (e.g. GridTracker) and searchlight together, you must configure WSJT-X, searchlight, and the 3rd party software to use a multicast IP address like 224.0.0.1.

```json
{
  "Server": {
    "Address": "127.0.0.1",
    "Port": 2237
  },
  "Palette": {
    "ReceptionReportBackgroundColors": [ "#114397", "#453f99", "#653897", "#812e91", "#991f87", "#ae027a", "#be006a", "#cb0058", "#d30044", "#d7002e" ],
    "ReceptionReportForegroundColor": "#ffff00",
    "ContactedBackgroundColor": "#000000",
    "ContactedForegroundColor": "#ffff00"
  },
  "PskReporter": {
    "ReportWindowSeconds": -900,
    "ReportRetrievalPeriodSeconds": 500
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "searchlight-log.txt",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}"
        }
      }
    ]
  }
}
```