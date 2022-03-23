# WsjtxUtils.Searchlight
A .NET 6 console application that highlights callsigns within [WSJT-X](https://physics.princeton.edu/pulsar/k1jt/wsjtx.html) based on reception reports from [PSK Reporter](https://pskreporter.info/).

> **NOTE:** This software is currently in a pre-release alpha state.


## Requirements
- [.NET 6 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/)


## Quickstart
```sh
git clone https://github.com/KC3PIB/WsjtxUtils.Searchlight

cd WsjtxUtils.Searchlight

dotnet restore WsjtxUtils.Searchlight.sln

dotnet build WsjtxUtils.Searchlight.sln

cd src/WsjtxUtils.Searchlight.Console/

dotnet run WsjtxUtils.Searchlight.Console.csproj
```