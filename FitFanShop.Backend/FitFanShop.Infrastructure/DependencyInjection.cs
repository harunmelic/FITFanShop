using FitFanShop.Application.Abstractions;
using FitFanShop.Infrastructure.Common;
using FitFanShop.Infrastructure.Database;
using FitFanShop.Infrastructure.Database.Interceptors;
using FitFanShop.Infrastructure.Services;
using FitFanShop.Shared.Constants;
using FitFanShop.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
namespace FitFanShop.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            if (env.IsTest())
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");
                return;
            }
            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.Main;
            options.UseSqlServer(cs)
                   .AddInterceptors(
                       new ProductCategoryValidationInterceptor(),
                       new StockReductionInterceptor(),
                       new ProductVariantCascadeDeleteInterceptor());
        });
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());
        services.AddScoped<IPasswordHasher<FitFanShopUserEntity>, PasswordHasher<FitFanShopUserEntity>>();
        services.AddTransient<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IAppCurrentUser, AppCurrentUser>();
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        return services;
    }
}