using Microsoft.AspNetCore.Components;

namespace ALib.BlazorWasmSimpleAuth;

public class RedirectToLogin : ComponentBase
{
    [Inject] public NavigationManager Navigation { get; set; } = null!;
    [Inject] public SimpleAuthenticationStateProvider Auth { get; set; } = null!;
    [Parameter] public string Url { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await Auth.LoadFromStorageAsync();
        if (Auth.User == null)
        {
            Navigation.NavigateTo(Url);
        }
    }
}
