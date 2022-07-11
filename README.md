
# WsjtxUtils.Searchlight
A .NET 6 console application that highlights callsigns within [WSJT-X](https://physics.princeton.edu/pulsar/k1jt/wsjtx.html) based on reception reports from [PSK Reporter](https://pskreporter.info/).

WjtxUtils.Searchlight will download reception reports from PSK Reporter for the callsigns of any connected WSJT-X client. These reception reports are then correlated with recent decodes and highlighted within WSJT-X. The background color of the highlighted callsign is mapped to the range of received reception report SNR values and used to indicate the relative strength of the received signal.

When a logged QSO occurs, that callsign will be highlighted for the duration of the application and can be optionally logged to a file to allow for tracking logged QSOs between sessions.

There is a countdown period when the application starts until requesting the first reception report, which defaults to 5 minutes. This period is an excellent time to call CQ for a while to seed the initial reception report.

> **NOTE:** To use 3rd party software ([GridTracker](https://gridtracker.org/grid-tracker/)) or multiple WSJT-X instances (SO2R), you must configure WSJT-X, searchlight, and any 3rd party software to use a [multicast address](https://en.wikipedia.org/wiki/Multicast_address).

## Requirements
For pre-compiled [releases](https://github.com/KC3PIB/WsjtxUtils.Searchlight/releases):
- [.NET 6 Runtime](https://docs.microsoft.com/en-us/dotnet/core/install/)
    - Installation instruction for [Windows](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net60), [Mac](https://docs.microsoft.com/en-us/dotnet/core/install/macos), [Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux)

To compile from source:
- [.NET 6 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/)
    - Installation instruction for [Windows](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net60), [Mac](https://docs.microsoft.com/en-us/dotnet/core/install/macos), [Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux)

## Quickstart
For pre-compiled [releases](https://github.com/KC3PIB/WsjtxUtils.Searchlight/releases) (win-x64, linux-x64, osx-x64):
- Ensure that you have the .NET 6 runtime installed
- Download the package for your OS
- Extract the archive to your desired location
- Run the executable file
    - Windows ```WjtxUtils.Searchlight.Console.exe```
    - Linux & OSX ```WsjtxUtils.Searchlight.Console```
- CTRL-C in the console to exit
- Edit the config.json file to customize available options

To compile from source:
- Ensure that you have the .NET 6 SDK installed
- Clone or download the source code
- ```dotnet publish -c release src/WsjtxUtils.Searchlight.Console/WsjtxUtils.Searchlight.Console.csproj --output <OUTPUT_DIRECTORY>```

## Configuration
Options can be configured by editing the [config.json](https://github.com/KC3PIB/WsjtxUtils.Searchlight/blob/development/src/WsjtxUtils.Searchlight.Console/config.json) file or by overriding specific parameters by command-line. These options include WSJT-X server IP address and port, the colors used for highlighted callsigns, reception report window/request period, and logging options.

To change the UDP server IP Address or port to listen for messages from WSJT-X
```json
"Server": {
    "Address": "127.0.0.1",
    "Port": 2237
  }
```
To use 3rd party software ([GridTracker](https://gridtracker.org/grid-tracker/)) or operate more than one WSJT-X instance (SO2R) use a [multicast address](https://en.wikipedia.org/wiki/Multicast_address).
```json
"Server": {
    "Address": "224.0.0.1",
    "Port": 2237
  }
```
The color options used for reception reports and logged QSOs are altered through the Palette section. ```ReceptionReportBackgroundColors``` is a list of colors, [a gradient](https://colordesigner.io/gradient-generator), used as the background color for reception reports. The first color in the list represents the weakest SNR report values and the last the strongest SNR values. ```ReceptionReportForegroundColor``` controls the color used for highlighted text (callsigns). Both ```ContactedBackgroundColor``` and ```ContactedForegroundColor``` are used for logged QSOs. ```HighlightCallsignsPeriodSeconds``` is the period at which callsigns will be correlated and highlighted based on the current reception report.
```json
"Palette": {
    "ReceptionReportBackgroundColors": [ "#114397", "#453f99", "#653897", "#812e91", "#991f87", "#ae027a", "#be006a", "#cb0058", "#d30044", "#d7002e" ],
    "ReceptionReportForegroundColor": "#ffff00",
    "ContactedBackgroundColor": "#000000",
    "ContactedForegroundColor": "#ffff00",
    "HighlightCallsignsPeriodSeconds": 5
  }
```
Reception report options are altered through the ```PskReporter``` section. ```ReportWindowSeconds``` is a negative number in seconds to indicate how much data to retrieve. This value cannot be more than 24 hours and defaults to -900 seconds or the previous 15 minutes, which should be enough data for current band conditions. ```ReportRetrievalPeriodSeconds``` controls how often reception reports are retrieved. Philip from [PSK Reporter](https://pskreporter.info/) has asked to limit requests to once every five minutes. IMHO Philip does a considerable service to the ham radio community with this data, don't abuse it.
```json
"PskReporter": {
    "ReportWindowSeconds": -900,
    "ReportRetrievalPeriodSeconds": 300
  }
```
The ```LoggedQsos``` section allows adding an optional log file to maintain logged QSOs across searchlight sessions. Set a ```LogFilePath```  to enable QSO logging. ```QsoManagerBehavior``` controls how logged QSOs are highlighted, once per band or once per band and mode.
```json
"LoggedQsos": {
    "LogFilePath": "qsolog.txt",
    "QsoManagerBehavior": "OncePerBand"
  }
```
Console and file logging output is controlled through the ```Serilog``` section. Please see the [Serilog documentation](https://github.com/serilog/serilog-settings-configuration) for details.
```json
"Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}"
        }
      }
    ]
  }
```