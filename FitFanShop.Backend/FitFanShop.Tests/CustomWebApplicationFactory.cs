using FitFanShop.Application.Modules.Auth.Commands.Login;
using FitFanShop.Infrastructure.Database;
using FitFanShop.Domain.Entities.Sales;
using FitFanShop.Application.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FitFanShop.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program>
{
    private static string? _cachedToken;
    private static bool _seeded = false;
    private static readonly object _lock = new object();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        
        builder.ConfigureServices(services =>
        {
            var descriptorDbContext = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            if (descriptorDbContext != null)
            {
                services.Remove(descriptorDbContext);
            }

            var descriptorInterceptors = services.Where(
                d => d.ServiceType == typeof(IInterceptor)).ToList();
            
            foreach (var descriptor in descriptorInterceptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            services.RemoveAll<IAppDbContext>();
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<DatabaseContext>());
        });
    }

    public async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        HttpClient? client = null;
        
        for (int i = 0; i < 3; i++)
        {
            try
            {
                client = CreateClient();
                break;
            }
            catch (InvalidOperationException)
            {
                if (i == 2) throw;
                await Task.Delay(100);
            }
        }

        if (client == null)
            throw new InvalidOperationException("Failed to create client after 3 attempts");
        
        EnsureSeeded();
        
        if (string.IsNullOrEmpty(_cachedToken))
        {
            var loginRequest = new
            {
                Email = "admin@fitfanshop.com",
                Password = "admin123"
            };

            var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginCommandDto>();
            if (loginResponse == null)
                throw new Exception("Login response is null.");
            _cachedToken = loginResponse.AccessToken;
        }
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _cachedToken);
        return client;
    }

    private void EnsureSeeded()
    {
        if (_seeded) return;

        lock (_lock)
        {
            if (_seeded) return;

            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                db.Database.EnsureCreated();

                if (!db.OrderStatuses.Any())
                {
                    db.OrderStatuses.AddRange(
                        new OrderStatusEntity { Name = "Pending" },
                        new OrderStatusEntity { Name = "Confirmed" },
                        new OrderStatusEntity { Name = "Delivered" },
                        new OrderStatusEntity { Name = "Cancelled" }
                    );
                    db.SaveChanges();
                }

                _seeded = true;
            }
            catch
            {
                _seeded = false;
            }
        }
    }
}