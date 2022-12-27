using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ALib.BlazorWasmSimpleAuth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleAuth(this IServiceCollection services)
    {
        services.AddBlazoredSessionStorage();
        services.AddScoped<SimpleAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<SimpleAuthenticationStateProvider>());
        services.AddAuthorizationCore();
        return services;
    }
}
