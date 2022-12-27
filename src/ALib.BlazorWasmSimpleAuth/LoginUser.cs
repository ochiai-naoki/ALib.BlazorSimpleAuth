namespace ALib.BlazorWasmSimpleAuth;

public class LoginUser
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
}
