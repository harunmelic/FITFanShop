using FitFanShop.API;
using FitFanShop.API.Middleware;
using FitFanShop.Application;
using FitFanShop.Infrastructure;
using FitFanShop.Shared.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting FitFanShop API...");

    builder.Host.UseSerilog((ctx, services, cfg) =>
    {
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext()
           .Enrich.WithThreadId()
           .Enrich.WithProcessId()
           .Enrich.WithMachineName();
    });

    builder.Logging.ClearProviders();

    builder.Services
        .AddAPI(builder.Configuration, builder.Environment)
        .AddInfrastructure(builder.Configuration, builder.Environment)
        .AddApplication();

    builder.Services.Configure<CorsOptions>(builder.Configuration.GetSection("Cors"));
    builder.Services.Configure<LocalizationOptions>(builder.Configuration.GetSection("Localization"));

    var corsOptions = builder.Configuration.GetSection("Cors").Get<CorsOptions>();
    if (corsOptions == null || corsOptions.AllowedOrigins == null)
        throw new InvalidOperationException("CORS configuration is missing or invalid.");
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularDev", policy =>
        {
            policy.WithOrigins(corsOptions.AllowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    var localizationOptions = builder.Configuration.GetSection("Localization").Get<LocalizationOptions>();
    if (localizationOptions == null || localizationOptions.DefaultCulture == null || localizationOptions.SupportedCultures == null)
        throw new InvalidOperationException("Localization configuration is missing or invalid.");
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.SetDefaultCulture(localizationOptions.DefaultCulture)
               .AddSupportedCultures(localizationOptions.SupportedCultures)
               .AddSupportedUICultures(localizationOptions.SupportedCultures);
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseExceptionHandler();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();

    app.UseRequestLocalization();

    app.UseHttpsRedirection();
    app.UseCors("AllowAngularDev");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.Services.InitializeDatabaseAsync(app.Environment);

    Log.Information("FitFanShop API started successfully.");
    app.Run();
}
catch (HostAbortedException)
{
    Log.Information("Host aborted by EF Core tooling (design-time) - its ok.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "FitFanShop API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
