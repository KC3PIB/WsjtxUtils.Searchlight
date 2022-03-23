// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Serilog;
using WsjtxUtils.Searchlight.Common;
using WsjtxUtils.Searchlight.Common.Settings;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("config.json", true, false)
    .AddCommandLine(args)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

await new Searchlight(configuration.Get<SearchlightSettings>()).RunAsync(GenerateCancellationTokenSource());

Log.CloseAndFlush();

/// <summary>
/// Creates a <see cref="CancellationTokenSource"/> which will signal
/// the task cancellation on pressing CTRL-C in the console application
/// </summary>
/// <returns></returns>
static CancellationTokenSource GenerateCancellationTokenSource()
{
    var cancellationTokenSource = new CancellationTokenSource();
    Console.CancelKeyPress += (s, e) =>
    {
        Log.Information("CTRL-C Canceling...");
        cancellationTokenSource.Cancel();
        e.Cancel = true;
    };
    return cancellationTokenSource;
}