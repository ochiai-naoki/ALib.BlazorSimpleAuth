# ALib.BlazorSimpleAuth

Blazor では `AuthenticationStateProvider` を継承して認証と承認を行います。

Blazor Server では `AuthenticationStateProvider` を継承した `ALib.BlazorServerSimpleAuth.SimpleAuthenticationStateProvider` を使用します。
Blazor Wasm では `ALib.BlazorWasmSimpleAuth.SimpleAuthenticationStateProvider` を使用します。

それぞれの使用方法等は Sample を参照してください。

`ALib.BlazorServerSimpleAuth` を使用した例を Sample を元に説明します。

*Program.cs*
```csharp
using ALib.BlazorServerSimpleAuth;

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMudServices();
builder.Services.AddSimpleAuth();
```
`AddSimpleAuth()` で `SimpleAuthenticationStateProvider` をDIに追加します。


*_imports.razor*
```html
@attribute [Authorize]
@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MudBlazor
@using BlazorServerSample
@using BlazorServerSample.Shared
```
`@attribute [Authorize]` で全てのページを認証済みでないとアクセスできないようにします。


*App.razor*
```html
@using ALib.BlazorServerSimpleAuth
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    <RedirectToLogin Url="/login" />
                </NotAuthorized>
            </AuthorizeRouteView>
        </Found>
        <NotFound>
            <PageTitle>Not found</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```
`CascadingAuthenticationState` で囲み、`RouteView` を `AuthorizeRouteView` に変更します。
`NotAuthorized` で未認証の場合に `RedirectToLogin` でログインページに飛ばすようにします。


*MainLayout.razor*
```html
@inherits LayoutComponentBase
@inject ALib.BlazorServerSimpleAuth.SimpleAuthenticationStateProvider Auth
@inject IDialogService DialogService

<MudThemeProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar Elevation="0">
        <MudIconButton Icon="@Icons.Material.Filled.Menu" Color="Color.Inherit" Edge="Edge.Start" OnClick="@((e) => DrawerToggle())" />
        <MudSpacer />
        @if (Auth.User != null)
        {
            <MudButton Color="Color.Inherit" EndIcon="@Icons.Material.Outlined.Logout" OnClick="Logout">@Auth.User?.UserName</MudButton>
        }
        <MudIconButton Icon="@Icons.Custom.Brands.GitHub" Color="Color.Inherit" Link="https://github.com/ochiai-naoki/ALib.BlazorSimpleAuth/" Target="_blank" />
    </MudAppBar>
    <MudDrawer @bind-Open="_drawerOpen" Elevation="1">
        <MudDrawerHeader>
            <MudText Typo="Typo.h6">BlazorServerSample</MudText>
        </MudDrawerHeader>
        <NavMenu />
    </MudDrawer>
    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.Large" Class="my-16 pt-16">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>

@code {
    bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    async void Logout()
    {
        var result = await DialogService.ShowMessageBox(
            "確認",
            "ログアウトします。",
            cancelText: "キャンセル"
        );
        if (result == true)
        {
            await Auth.LogoutAsync();
        }
        StateHasChanged();
    }
}
```
`ALib.BlazorServerSimpleAuth.SimpleAuthenticationStateProvider` をインジェクションします。
認証済みの場合は `User` プロパティに認証済みユーザーが設定されています。
AppBar に認証済みユーザー名を表示し、クリックするとログアウトできるようにします。


*Login.razor*
```html
@page "/login"
@layout LoginLayout
@attribute [AllowAnonymous]
@using System.ComponentModel.DataAnnotations;
@using ALib.BlazorServerSimpleAuth
@inject SimpleAuthenticationStateProvider Auth
@inject NavigationManager Navigation

<EditForm Model="_user" OnValidSubmit="OnValidSubmit">
    <DataAnnotationsValidator />
    <MudCard>
        <MudCardContent>
            <MudStack>
                <MudTextField Label="ユーザーID" @bind-Value="_user.UserId" For="@(() => _user.UserId)" />
                <MudTextField Label="パスワード" @bind-Value="_user.Password" For="@(() => _user.Password)" InputType="InputType.Password" />
                @if (!string.IsNullOrEmpty(_message))
                {
                    <MudAlert Severity="Severity.Error">@_message</MudAlert>
                }
            </MudStack>
        </MudCardContent>
        <MudCardActions>
            <MudButton ButtonType="ButtonType.Submit">ログイン</MudButton>
        </MudCardActions>
    </MudCard>
</EditForm>

<MudAlert Class="mt-8">
    <MudText>ユーザーID: 'a' ... Admin (Counter ページでリセットが可能)</MudText>
    <MudText>ユーザーID: 'b' ... Guest</MudText>
    <MudText Color="Color.Secondary">パスワードは何でもOK</MudText>
</MudAlert>

@code {
    LoginUserModel _user = new();
    string? _message;

    private async Task OnValidSubmit(EditContext context)
    {
        _message = null;
        var user = _user.UserId switch
        {
            "a" => new LoginUser
                {
                    UserId = "a",
                    UserName = "アドミン",
                    Roles = new[] { "Admin" }
                },
            "b" => new LoginUser
                {
                    UserId = "b",
                    UserName = "ゲスト",
                    Roles = new[] { "Guest" }
                },
            _ => null
        };
        if (user != null)
        {
            await Auth.LoginAsync(user);
            Navigation.NavigateTo("/");
        }
        StateHasChanged();
    }

    class LoginUserModel
    {
        [Required]
        public string? UserId { get; set; }
        public string? Password { get; set; }
    }
}
```
`@attribute [AllowAnonymous]` で未認証でもアクセスできるようにします。
認証が完了したら `LoginUser` インスタンスを作成し、`SimpleAuthenticationStateProvider` の `LoginAsync` を呼び出します。
ログインページではサイドバーを出さないように `@layout` で専用のレイアウトに切り替えています。


*Counter.razor*
```html
@page "/counter"

<PageTitle>Counter</PageTitle>

<MudText Typo="Typo.h3" GutterBottom="true">Counter</MudText>
<MudText Class="mb-4">Current count: @currentCount</MudText>
<MudButton Color="Color.Primary" Variant="Variant.Filled" @onclick="IncrementCount">Click me</MudButton>
<AuthorizeView Roles="Admin">
    <MudButton Color="Color.Secondary" Variant="Variant.Filled" @onclick="ResetCount">Reset</MudButton>
</AuthorizeView>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }

    private void ResetCount()
    {
        currentCount = 0;
    }
}

```
`AuthorizeView` で Admin のロールを持っているユーザーだけがカウンターをリセットできるようにリセットボタンを追加しました。

