using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ALib.BlazorServerSimpleAuth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleAuth(this IServiceCollection services)
    {
        services.AddScoped<SimpleAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<SimpleAuthenticationStateProvider>());
        return services;
    }
}
