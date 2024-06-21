using IdentityServer.AspNetCore;
using Serilog;
using TestDuende.IdentityServer;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.ConfigureIdentityServer();

    var app = builder.Build();
    
    app.ConfigurePipeline();

    if (args.Contains("/seed"))
    {
        Log.Information("Seeding database...");
        SeedData.EnsureSeedDataConfiguration(app);
        SeedData.EnsureSeedDataUsers(app);
        Log.Information("Done seeding database. Exiting.");
        return;
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}