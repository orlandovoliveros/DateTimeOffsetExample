using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DateTimeOffsetExample
{
    // https://docs.microsoft.com/en-us/dotnet/standard/datetime/choosing-between-datetime
    class Program
    {
        private static ILogger<Program> _logger;

        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    _logger = services.GetRequiredService<ILogger<Program>>();

                    var dateTimeNow = DateTime.Now;
                    var dateTimeUtcNow = DateTime.UtcNow;
                    var dateTimeOffsetNow = DateTimeOffset.Now;
                    var dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;

                    _logger.LogInformation("DateTime.Now Date                   {dt:yyyy-MM-dd HH:mm:ss.fff}", dateTimeNow.Date);
                    _logger.LogInformation("DateTime.Now                        {dt:yyyy-MM-dd HH:mm:ss.fff}", dateTimeNow);

                    _logger.LogInformation("DateTime.UtcNow Date                {dt:yyyy-MM-dd HH:mm:ss.fff}", dateTimeUtcNow.Date);
                    _logger.LogInformation("DateTime.UtcNow                     {dt:yyyy-MM-dd HH:mm:ss.fff}", dateTimeUtcNow);

                    _logger.LogInformation("DateTimeOffset.Now Date             {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetNow.Date);
                    _logger.LogInformation("DateTimeOffset.Now DateTime         {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetNow.DateTime);
                    _logger.LogInformation("DateTimeOffset.Now LocalDateTime    {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetNow.LocalDateTime);
                    _logger.LogInformation("DateTimeOffset.Now UtcDataTime      {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetNow.UtcDateTime);

                    _logger.LogInformation("DateTimeOffset.UtcNow Date          {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetUtcNow.Date);
                    _logger.LogInformation("DateTimeOffset.UtcNow DateTime      {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetUtcNow.DateTime);
                    _logger.LogInformation("DateTimeOffset.UtcNow LocalDateTime {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetUtcNow.LocalDateTime);
                    _logger.LogInformation("DateTimeOffset.UtcNow UtcDataTime   {dto:yyyy-MM-dd HH:mm:ss.fff}", dateTimeOffsetUtcNow.UtcDateTime);

                    ShowPossibleTimeZones(dateTimeOffsetNow);

                    ShowPossibleTimeZones(dateTimeOffsetUtcNow);
                }
                catch (Exception)
                {

                }
            }

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostBuilderContext, services) =>
            {
            })
            .UseSerilog((hostBuilderContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ({Application}) ({SourceContext}) {Message:lj}{NewLine}{Exception}",
                        theme: SystemConsoleTheme.Literate)
                    .MinimumLevel.ControlledBy(new LoggingLevelSwitch { MinimumLevel = LogEventLevel.Verbose })
                    .MinimumLevel.Override("DateTimeOffsetExample.Program", LogEventLevel.Verbose)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "DateTimeOffset Example");
            });

        private static void ShowPossibleTimeZones(DateTimeOffset offsetTime)
        {
            TimeSpan offset = offsetTime.Offset;
            ReadOnlyCollection<TimeZoneInfo> timeZones;

            _logger.LogInformation("{0:yyyy-MM-dd HH:mm:ss.fff} could belong to the following time zones:", offsetTime);

            // Get all time zones defined on local system
            timeZones = TimeZoneInfo.GetSystemTimeZones();

            // Iterate time zones
            foreach (TimeZoneInfo timeZone in timeZones)
            {
                // Compare offset with offset for that date in that time zone
                if (timeZone.GetUtcOffset(offsetTime.DateTime).Equals(offset))
                {
                    _logger.LogInformation("   {0}", timeZone.DisplayName);
                }
            }

            _logger.LogInformation("");
        }
    }
}
